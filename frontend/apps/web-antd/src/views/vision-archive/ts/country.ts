import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import * as echarts from 'echarts';

const router = useRouter();
const route = useRoute();

const DARK_COLOR = {
  primary: '#38bdf8', secondary: '#a78bfa', accent: '#f472b6',
  warning: '#fbbf24', success: '#34d399', danger: '#fb7185',
  male: '#38bdf8', female: '#a78bfa',
  text: '#e2e8f0', textDim: '#94a3b8',
  bg: 'rgba(10, 22, 50, 0.96)', mapArea: 'rgba(15, 30, 65, 0.85)',
  mapBorder: 'rgba(56, 189, 248, 0.75)', mapLabel: '#e2e8f0',
  splitLine: 'rgba(56, 189, 248, 0.12)',
  gaugeTrack: 'rgba(56, 189, 248, 0.12)', initialBar: '#475569',
  mapColors: ['#0891b2', '#38bdf8', '#a78bfa', '#f472b6'],
};

const LIGHT_COLOR = {
  primary: '#0891b2', secondary: '#7c3aed', accent: '#db2777',
  warning: '#d97706', success: '#059669', danger: '#dc2626',
  male: '#0891b2', female: '#7c3aed',
  text: '#1e293b', textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)', mapArea: '#f1f5f9',
  mapBorder: 'rgba(8, 145, 178, 0.35)', mapLabel: '#1e293b',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)', initialBar: '#cbd5e1',
  mapColors: ['#67e8f9', '#0891b2', '#7c3aed', '#db2777'],
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 }, { offset: 1, color: c2 },
  ]);

const COUNTY_MAP_CONFIG = {
  '370102': { name: '历下区', short: '历下区', center: [117.14, 36.65], zoom: 2.3 },
  '370103': { name: '市中区', short: '市中区', center: [117.10, 36.63], zoom: 2.3 },
  '370104': { name: '槐荫区', short: '槐荫区', center: [116.98, 36.65], zoom: 2.3 },
  '370105': { name: '天桥区', short: '天桥区', center: [117.02, 36.68], zoom: 2.3 },
  '370112': { name: '历城区', short: '历城区', center: [117.14, 36.67], zoom: 2.2 },
  '370113': { name: '长清区', short: '长清区', center: [116.75, 36.55], zoom: 2.2 },
  '370702': { name: '潍城区', short: '潍城区', center: [119.16, 36.71], zoom: 2.3 },
  '370704': { name: '坊子区', short: '坊子区', center: [119.17, 36.62], zoom: 2.3 },
  '370705': { name: '奎文区', short: '奎文区', center: [119.12, 36.71], zoom: 2.3 },
  '370703': { name: '寒亭区', short: '寒亭区', center: [119.16, 36.78], zoom: 2.3 },
};

const DISTRICT_SCHOOLS_MAP = {
  '370102': [
    '济南市历下区第一实验小学', '济南市历下区实验中学', '济南市历下区东方双语学校',
    '济南市历下区第二实验小学', '济南市历下区育英中学', '济南市历下区汇文学校',
    '济南市历下区燕翔小学', '济南市历下区济南五中', '济南市历下区解放路第一小学',
    '济南市历下区山大路小学', '济南市历下区文化东路小学', '济南市历下区建筑新村学校'
  ],
  '370103': [
    '济南市市中区经五路小学', '济南市市中区实验中学', '济南市市中区胜利大街小学',
    '济南市市中区济南中学', '济南市市中区育英小学', '济南市市中区十四中学',
    '济南市市中区南上山街小学', '济南市市中区七贤中学', '济南市市中区舜耕小学',
    '济南市市中区济南十六里河中学', '济南市市中区七里山小学', '济南市市中区党家镇中心小学'
  ],
  '370104': [
    '济南市槐荫区经十路小学', '济南市槐荫区实验中学', '济南市槐荫区南辛庄小学',
    '济南市槐荫区济南中学分校', '济南市槐荫区匡山小学', '济南市槐荫区二十七中',
    '济南市槐荫区张庄小学', '济南市槐荫区二机床学校', '济南市槐荫区古城中学',
    '济南市槐荫区段店小学'
  ],
  '370105': [
    '济南市天桥区无影山小学', '济南市天桥区实验中学', '济南市天桥区工人新村小学',
    '济南市天桥区十一中', '济南市天桥区堤口路小学', '济南市天桥区二十九中',
    '济南市天桥区师范路小学', '济南市天桥区双语实验学校', '济南市天桥区北坦小学',
    '济南市天桥区清河小学'
  ],
  '370112': [
    '济南市历城区洪家楼小学', '济南市历城区实验中学', '济南市历城区第二实验小学',
    '济南市历城区历城二中', '济南市历城区外国语学校', '济南市历城区大辛庄小学',
    '济南市历城区董家镇中心小学', '济南市历城区华山中学', '济南市历城区王舍人镇小学',
    '济南市历城区仲宫镇中心小学', '济南市历城区唐王镇小学', '济南市历城区郭店镇中心学校'
  ],
  '370113': [
    '济南市长清区实验小学', '济南市长清区实验中学', '济南市长清区第一中学',
    '济南市长清区石麟小学', '济南市长清区第二中学', '济南市长清区马山镇小学',
    '济南市长清区双泉镇中心小学', '济南市长清区张夏镇中学', '济南市长清区万德镇中心学校',
    '济南市长清区五峰山小学'
  ],
  '370702': [
    '潍坊市潍城区实验小学', '潍坊市潍城区实验中学', '潍坊市潍城区东风小学',
    '潍坊市潍城区潍坊七中', '潍坊市潍城区月河路小学', '潍坊市潍城区三中',
    '潍坊市潍城区永安路小学', '潍坊市潍城区芙蓉小学', '潍坊市潍城区健康街小学',
    '潍坊市潍城区西关小学'
  ],
  '370704': [
    '潍坊市坊子区实验小学', '潍坊市坊子区实验中学', '潍坊市坊子区第二实验小学',
    '潍坊市坊子区潍坊四中', '潍坊市坊子区凤凰小学', '潍坊市坊子区三马路小学',
    '潍坊市坊子区九龙涧学校', '潍坊市坊子区坊城小学', '潍坊市坊子区坊安小学',
    '潍坊市坊子区黄旗堡小学'
  ],
  '370705': [
    '潍坊市奎文区实验小学', '潍坊市奎文区实验中学', '潍坊市奎文区胜利东小学',
    '潍坊市奎文区广文中学', '潍坊市奎文区德信现代学校', '潍坊市奎文区新华中学',
    '潍坊市奎文区圣荣小学', '潍坊市奎文区东关育才小学', '潍坊市奎文区友谊小学',
    '潍坊市奎文区北苑实验学校'
  ],
  '370703': [
    '潍坊市寒亭区实验小学', '潍坊市寒亭区实验中学', '潍坊市寒亭区第一中学',
    '潍坊市寒亭区第二实验小学', '潍坊市寒亭区潍坊一中', '潍坊市寒亭区外国语学校',
    '潍坊市寒亭区朱里镇中心小学', '潍坊市寒亭区固堤镇小学', '潍坊市寒亭区高里镇学校',
    '潍坊市寒亭区央子镇中心小学'
  ],
};

const SCHOOL_CODES_MAP = {
  '济南市历下区第一实验小学': '370102001', '济南市历下区实验中学': '370102002',
  '济南市历下区东方双语学校': '370102003', '济南市历下区第二实验小学': '370102004',
  '济南市历下区育英中学': '370102005', '济南市历下区汇文学校': '370102006',
  '济南市历下区燕翔小学': '370102007', '济南市历下区济南五中': '370102008',
  '济南市历下区解放路第一小学': '370102009', '济南市历下区山大路小学': '370102010',
  '济南市历下区文化东路小学': '370102011', '济南市历下区建筑新村学校': '370102012',
  '济南市市中区经五路小学': '370103001', '济南市市中区实验中学': '370103002',
  '济南市市中区胜利大街小学': '370103003', '济南市市中区济南中学': '370103004',
  '济南市市中区育英小学': '370103005', '济南市市中区十四中学': '370103006',
  '济南市市中区南上山街小学': '370103007', '济南市市中区七贤中学': '370103008',
  '济南市市中区舜耕小学': '370103009', '济南市市中区济南十六里河中学': '370103010',
  '济南市市中区七里山小学': '370103011', '济南市市中区党家镇中心小学': '370103012',
  '济南市槐荫区经十路小学': '370104001', '济南市槐荫区实验中学': '370104002',
  '济南市槐荫区南辛庄小学': '370104003', '济南市槐荫区济南中学分校': '370104004',
  '济南市槐荫区匡山小学': '370104005', '济南市槐荫区二十七中': '370104006',
  '济南市槐荫区张庄小学': '370104007', '济南市槐荫区二机床学校': '370104008',
  '济南市槐荫区古城中学': '370104009', '济南市槐荫区段店小学': '370104010',
  '济南市天桥区无影山小学': '370105001', '济南市天桥区实验中学': '370105002',
  '济南市天桥区工人新村小学': '370105003', '济南市天桥区十一中': '370105004',
  '济南市天桥区堤口路小学': '370105005', '济南市天桥区二十九中': '370105006',
  '济南市天桥区师范路小学': '370105007', '济南市天桥区双语实验学校': '370105008',
  '济南市天桥区北坦小学': '370105009', '济南市天桥区清河小学': '370105010',
  '济南市历城区洪家楼小学': '370112001', '济南市历城区实验中学': '370112002',
  '济南市历城区第二实验小学': '370112003', '济南市历城区历城二中': '370112004',
  '济南市历城区外国语学校': '370112005', '济南市历城区大辛庄小学': '370112006',
  '济南市历城区董家镇中心小学': '370112007', '济南市历城区华山中学': '370112008',
  '济南市历城区王舍人镇小学': '370112009', '济南市历城区仲宫镇中心小学': '370112010',
  '济南市历城区唐王镇小学': '370112011', '济南市历城区郭店镇中心学校': '370112012',
  '济南市长清区实验小学': '370113001', '济南市长清区实验中学': '370113002',
  '济南市长清区第一中学': '370113003', '济南市长清区石麟小学': '370113004',
  '济南市长清区第二中学': '370113005', '济南市长清区马山镇小学': '370113006',
  '济南市长清区双泉镇中心小学': '370113007', '济南市长清区张夏镇中学': '370113008',
  '济南市长清区万德镇中心学校': '370113009', '济南市长清区五峰山小学': '370113010',
  '潍坊市潍城区实验小学': '370702001', '潍坊市潍城区实验中学': '370702002',
  '潍坊市潍城区东风小学': '370702003', '潍坊市潍城区潍坊七中': '370702004',
  '潍坊市潍城区月河路小学': '370702005', '潍坊市潍城区三中': '370702006',
  '潍坊市潍城区永安路小学': '370702007', '潍坊市潍城区芙蓉小学': '370702008',
  '潍坊市潍城区健康街小学': '370702009', '潍坊市潍城区西关小学': '370702010',
  '潍坊市坊子区实验小学': '370704001', '潍坊市坊子区实验中学': '370704002',
  '潍坊市坊子区第二实验小学': '370704003', '潍坊市坊子区潍坊四中': '370704004',
  '潍坊市坊子区凤凰小学': '370704005', '潍坊市坊子区三马路小学': '370704006',
  '潍坊市坊子区九龙涧学校': '370704007', '潍坊市坊子区坊城小学': '370704008',
  '潍坊市坊子区坊安小学': '370704009', '潍坊市坊子区黄旗堡小学': '370704010',
  '潍坊市奎文区实验小学': '370705001', '潍坊市奎文区实验中学': '370705002',
  '潍坊市奎文区胜利东小学': '370705003', '潍坊市奎文区广文中学': '370705004',
  '潍坊市奎文区德信现代学校': '370705005', '潍坊市奎文区新华中学': '370705006',
  '潍坊市奎文区圣荣小学': '370705007', '潍坊市奎文区东关育才小学': '370705008',
  '潍坊市奎文区友谊小学': '370705009', '潍坊市奎文区北苑实验学校': '370705010',
  '潍坊市寒亭区实验小学': '370703001', '潍坊市寒亭区实验中学': '370703002',
  '潍坊市寒亭区第一中学': '370703003', '潍坊市寒亭区第二实验小学': '370703004',
  '潍坊市寒亭区潍坊一中': '370703005', '潍坊市寒亭区外国语学校': '370703006',
  '潍坊市寒亭区朱里镇中心小学': '370703007', '潍坊市寒亭区固堤镇小学': '370703008',
  '潍坊市寒亭区高里镇学校': '370703009', '潍坊市寒亭区央子镇中心小学': '370703010',
};

const SCHOOL_COORDS_MAP = {};
const _countyCenters = {
  '370102': [117.14, 36.65], '370103': [117.10, 36.63], '370104': [116.98, 36.65],
  '370105': [117.02, 36.68], '370112': [117.14, 36.67], '370113': [116.75, 36.55],
  '370702': [119.16, 36.71], '370704': [119.17, 36.62], '370705': [119.12, 36.71],
  '370703': [119.16, 36.78],
};
Object.entries(DISTRICT_SCHOOLS_MAP).forEach(([code, schools]) => {
  const center = _countyCenters[code] || [117.12, 36.65];
  schools.forEach((name, i) => {
    const angle = (i / schools.length) * Math.PI * 2;
    const radius = 0.04 + (i % 3) * 0.015;
    const lng = Math.round((center[0] + Math.cos(angle) * radius) * 1000) / 1000;
    const lat = Math.round((center[1] + Math.sin(angle) * radius) * 1000) / 1000;
    SCHOOL_COORDS_MAP[name] = [lng, lat];
  });
});

const PROVINCE_CODES = { '山东省': '370000' };

const PROVINCE_CITY_MAP = {
  '370000': [
    { code: '370100', name: '济南市' },
    { code: '370700', name: '潍坊市' },
  ],
};

const CITY_DISTRICT_MAP = {
  '370100': [
    { code: '370102', name: '历下区' },
    { code: '370103', name: '市中区' },
    { code: '370104', name: '槐荫区' },
    { code: '370105', name: '天桥区' },
    { code: '370112', name: '历城区' },
    { code: '370113', name: '长清区' },
  ],
  '370700': [
    { code: '370702', name: '潍城区' },
    { code: '370704', name: '坊子区' },
    { code: '370705', name: '奎文区' },
    { code: '370703', name: '寒亭区' },
  ],
};

const PROVINCE_NAMES = Object.fromEntries(Object.entries(PROVINCE_CODES).map(([n, c]) => [c, n]));
const CITY_NAMES = {};
Object.entries(PROVINCE_CITY_MAP).forEach(([_, cities]) => {
  cities.forEach(c => { CITY_NAMES[c.code] = c.name; });
});
const DISTRICT_NAMES = {};
Object.entries(CITY_DISTRICT_MAP).forEach(([_, districts]) => {
  if (Array.isArray(districts)) districts.forEach(d => { DISTRICT_NAMES[d.code] = d.name; });
});

const routeCode = computed(() => String(route.params.code || '370102'));
const currentCountyConfig = computed(() => {
  if (COUNTY_MAP_CONFIG[routeCode.value]) return COUNTY_MAP_CONFIG[routeCode.value];
  const code = routeCode.value;
  return { name: code, short: code, center: [117.12, 36.65], zoom: 2.0 };
});
const currentCounty = computed(() => currentCountyConfig.value.short);
const currentCountyCenter = computed(() => currentCountyConfig.value.center);

const schoolList = computed(() => {
  const districtCode = currentFilter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || DISTRICT_SCHOOLS_MAP[routeCode.value] || [];
  if (currentFilter.school) {
    const schoolName = Object.keys(SCHOOL_CODES_MAP).find(k => SCHOOL_CODES_MAP[k] === currentFilter.school);
    if (schoolName) return [schoolName];
  }
  return list;
});

const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

const metricConfig = {
  vision: { name: '近视', baseRate: 55, trend: 2.0, maleFactor: 0.95, femaleFactor: 1.05, primaryFactor: 0.6, juniorFactor: 1.0, seniorFactor: 1.25 },
  oral: { name: '龋齿', baseRate: 40, trend: -1.0, maleFactor: 0.96, femaleFactor: 1.05, primaryFactor: 1.24, juniorFactor: 0.9, seniorFactor: 0.67 },
  mental: { name: '心理预警', baseRate: 16, trend: -0.6, maleFactor: 0.92, femaleFactor: 1.1, primaryFactor: 0.67, juniorFactor: 1.22, seniorFactor: 1.56 },
  weight: { name: '超重/肥胖', baseRate: 22, trend: 1.2, maleFactor: 1.12, femaleFactor: 0.88, primaryFactor: 0.63, juniorFactor: 1.17, seniorFactor: 1.33 },
  bone: { name: '骨密度偏低', baseRate: 14, trend: -0.4, maleFactor: 0.91, femaleFactor: 1.11, primaryFactor: 0.75, juniorFactor: 1.13, seniorFactor: 1.38 }
};

const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const mapLoaded = ref(false);
const mapZoom = ref(2.0);

const currentMetric = computed(() => metricConfig[activeTab.value]);

const currentFilter = reactive({ province: '', city: '', district: '', school: '' });

const openDropdown = ref(null);

const provinceList = computed(() => {
  return Object.entries(PROVINCE_CODES).map(([name, code]) => ({
    code, name,
  }));
});

const cityOptions = computed(() => PROVINCE_CITY_MAP[currentFilter.province] || []);

const districtOptions = computed(() => CITY_DISTRICT_MAP[currentFilter.city] || []);

const schoolOptions = computed(() => {
  const districtCode = currentFilter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || [];
  return list.map(name => ({ code: SCHOOL_CODES_MAP[name] || name, name }));
});

const getSelectedName = (list, value) => {
  if (!value) return '';
  const item = list.find(i => i.value === value || i.code === value);
  return item ? item.name : '';
};

const toggleDropdown = (key) => {
  openDropdown.value = openDropdown.value === key ? null : key;
};

const selectOption = (key, value) => {
  if (key === 'province') {
    currentFilter.province = value;
    currentFilter.city = '';
    currentFilter.district = '';
    currentFilter.school = '';
  } else if (key === 'city') {
    currentFilter.city = value;
    currentFilter.district = '';
    currentFilter.school = '';
  } else if (key === 'district') {
    currentFilter.district = value;
    currentFilter.school = '';
  } else if (key === 'school') {
    currentFilter.school = value;
    const code = String(value);
    if (code.length >= 6) {
      const provCode = code.substring(0, 2) + '0000';
      const cityCode = code.substring(0, 4) + '00';
      const distCode = code.substring(0, 6);
      if (PROVINCE_CITY_MAP[provCode]) {
        currentFilter.province = provCode;
        if (PROVINCE_CITY_MAP[provCode].some(c => c.code === cityCode)) {
          currentFilter.city = cityCode;
          if (CITY_DISTRICT_MAP[cityCode] && CITY_DISTRICT_MAP[cityCode].some(d => d.code === distCode)) {
            currentFilter.district = distCode;
          }
        }
      }
    }
  }
  openDropdown.value = null;
  renderAllCharts();
};

const clearSchool = () => { currentFilter.school = ''; };
const clearProvince = () => {
  currentFilter.province = '';
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
};
const clearCity = () => {
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
};
const clearDistrict = () => {
  currentFilter.district = '';
  currentFilter.school = '';
};

const handleDropdownClickOutside = (e) => {
  if (!e.target.closest('.filter-select')) {
    openDropdown.value = null;
  }
};

const computeCountyMetricData = () => {
  const m = metricConfig[activeTab.value];
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rate = Math.round((m.baseRate + (seed % 12 - 6)) * 10) / 10;
  const students = Math.round(30 + seed * 3);
  const schools = schoolList.value.length || 10;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const primary = Math.round(rate * m.primaryFactor * 10) / 10;
  const junior = Math.round(rate * m.juniorFactor * 10) / 10;
  const senior = Math.round(rate * m.seniorFactor * 10) / 10;
  const sortedSchools = [...schoolList.value].sort((a, b) => {
    const ra = Math.round((rate + (schoolList.value.indexOf(a) % 8 - 4)) * 10) / 10;
    const rb = Math.round((rate + (schoolList.value.indexOf(b) % 8 - 4)) * 10) / 10;
    return rb - ra;
  });
  const topSchool = sortedSchools[0] || '最高';
  const topRate = Math.round((rate + 4 + (seed % 6)) * 10) / 10;
  return { rate, students, schools, trend: m.trend, maleRate, femaleRate, primary, junior, senior, topSchool, topRate };
};

const currentCountyData = computed(() => computeCountyMetricData());

const rankingChartRef = ref(null);
const ageGenderChartRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const trendChartRef = ref(null);
const interventionChartRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};
let timer = null;
let themeObserver = null;

const applySystemTheme = () => {
  isLight.value = !document.documentElement.classList.contains('dark');
  setTimeout(() => renderAllCharts(), 300);
};

const observeTheme = () => {
  themeObserver = new MutationObserver(() => {
    const newIsLight = !document.documentElement.classList.contains('dark');
    if (newIsLight !== isLight.value) {
      isLight.value = newIsLight;
    }
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

const updateDateTime = () => {
  const now = new Date();
  currentDate.value = now.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'short' });
  currentTime.value = now.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
};

const switchTab = (tabKey) => {
  activeTab.value = tabKey;
  setTimeout(() => renderAllCharts(), 100);
};

const handleSearch = () => { openDropdown.value = null; renderAllCharts(); };
const handleReset = () => {
  currentFilter.province = '';
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
  openDropdown.value = null;
};

const goBack = () => {
  const code = routeCode.value;
  const cityCode = code.slice(0, 4) + '00';
  router.push({ path: `/vision/city/${cityCode}`, query: { tab: activeTab.value } });
};

const drillToSchool = (name) => {
  const code = SCHOOL_CODES_MAP[name];
  if (code) router.push({ path: `/vision/school/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到学校编码:', name);
};

const getTooltip = (trigger = 'axis') => {
  const c = getColors();
  return {
    trigger,
    backgroundColor: c.bg,
    borderColor: c.primary,
    borderWidth: 1,
    textStyle: { color: c.text, fontSize: 12 },
    extraCssText: isLight.value
      ? 'box-shadow: 0 8px 24px rgba(15,23,42,0.12); border-radius: 8px;'
      : 'box-shadow: 0 0 20px rgba(56,189,248,0.35); border-radius: 6px; backdrop-filter: blur(8px);',
  };
};

const disposeAll = () => {
  Object.values(chartInstances).forEach(c => { if (c && !c.isDisposed()) c.dispose(); });
  Object.keys(chartInstances).forEach(k => delete chartInstances[k]);
};

const initChart = (ref, key, option, onClick, retryCount = 0) => {
  if (!ref.value) return;
  try {
    const rect = ref.value.getBoundingClientRect();
    if (rect.width === 0 || rect.height === 0) {
      if (retryCount < 3) setTimeout(() => initChart(ref, key, option, onClick, retryCount + 1), 200);
      return;
    }
    const chart = echarts.init(ref.value);
    chartInstances[key] = chart;
    chart.setOption(option);
    if (onClick) chart.on('click', onClick);
    chart.resize();
  } catch (e) {}
};

const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();

  const allSchools = schoolList.value.length > 0
    ? schoolList.value
    : (DISTRICT_SCHOOLS_MAP[routeCode.value] || []);

  const baseRate = currentCountyData.value?.rate || 50;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const data = allSchools.map((name, i) => ({
    name,
    value: Math.round((baseRate + ((seed + i * 5) % 18 - 9)) * 10) / 10
  }));

  if (data.length === 0) return {};

  const values = data.map(d => d.value);
  const minV = Math.min(...values);
  const maxV = Math.max(...values);
  const cfg = currentCountyConfig.value;

  const scatterData = data.map(d => ({
    name: d.name,
    value: [...(SCHOOL_COORDS_MAP[d.name] || cfg.center), d.value],
  }));

  const lineData = data.map(d => ({
    coords: [cfg.center, (SCHOOL_COORDS_MAP[d.name] || cfg.center)],
    value: d.value,
    name: d.name,
  }));

  const arrowSymbol = 'path://M2,-10 L6,-2 L2,-2 L2,10 L-2,10 L-2,-2 L-6,-2 Z';

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        if (p.seriesType === 'effectScatter' || p.seriesType === 'scatter') {
          const v = p.value;
          return `<div style="font-weight:600">${p.name}</div>
            <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${v[2]}%</span></div>
            <div style="color:${c.textDim};font-size:11px;margin-top:4px">📍 坐标：${v[0].toFixed(2)}°E, ${v[1].toFixed(2)}°N</div>
            <div style="color:${c.accent};font-size:11px;margin-top:4px">点击查看详情 →</div>`;
        }
        if (p.seriesType === 'lines') {
          return `<div style="font-weight:600">${p.name}</div>
            <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>
            <div style="color:${c.textDim};font-size:11px;margin-top:4px">点击查看详情 →</div>`;
        }
        return `<div style="font-weight:600">${p.name}</div>`;
      },
    },
    visualMap: {
      show: true,
      orient: 'horizontal',
      right: 16,
      bottom: 10,
      itemWidth: 10,
      itemHeight: 80,
      textStyle: { color: c.textDim, fontSize: 10 },
      min: minV, max: maxV,
      inRange: { color: c.mapColors },
      text: ['高', '低'],
      calculable: false,
    },
    geo: {
      map: 'county',
      roam: true,
      selectedMode: false,
      layoutCenter: ['50%', '50%'],
      layoutSize: '95%',
      zoom: 1,
      label: {
        show: true,
        color: c.mapLabel,
        fontSize: 10,
        fontWeight: '500',
        textShadowColor: isLight.value ? 'rgba(0,0,0,0.6)' : 'rgba(0,0,0,0.8)',
        textShadowBlur: 4,
      },
      itemStyle: {
        borderColor: c.mapBorder,
        borderWidth: 1.2,
        areaColor: c.mapArea,
        shadowBlur: isLight.value ? 6 : 12,
        shadowColor: `${c.primary}44`,
      },
      emphasis: {
        label: { color: '#ffffff', fontSize: 12, fontWeight: 700 },
        itemStyle: {
          areaColor: isLight.value ? 'rgba(241, 245, 249, 0.85)' : 'rgba(56,189,248,0.55)',
          borderColor: isLight.value ? 'rgba(8,145,178,0.3)' : c.primary,
          borderWidth: 2,
          shadowBlur: 20,
          shadowColor: `${c.primary}aa`,
        },
      },
    },
    series: [
      {
        type: 'lines',
        coordinateSystem: 'geo',
        zlevel: 1,
        effect: {
          show: true,
          period: 5,
          trailLength: 0.3,
          symbol: 'arrow',
          symbolSize: 6,
          color: c.primary,
        },
        lineStyle: {
          color: c.primary,
          width: 1,
          opacity: 0.4,
          curveness: 0.15,
        },
        data: lineData,
      },
      {
        type: 'effectScatter',
        coordinateSystem: 'geo',
        data: scatterData,
        symbolSize: (val) => Math.max(10, Math.min(18, (val[2] || 0) / maxV * 18 + 6)),
        showEffectOn: 'render',
        rippleEffect: {
          brushType: 'stroke',
          scale: 4,
          period: 3,
        },
        label: {
          show: true,
          formatter: '{b}',
          position: 'right',
          distance: 6,
          color: c.text,
          fontSize: 10,
          fontWeight: '500',
          textShadowColor: isLight.value ? 'rgba(255,255,255,0.95)' : 'rgba(0,0,0,0.85)',
          textShadowBlur: 4,
        },
        itemStyle: {
          color: c.primary,
          shadowBlur: 15,
          shadowColor: c.primary,
          borderColor: isLight.value ? '#fff' : c.bg,
          borderWidth: 2,
        },
        emphasis: {
          scale: 1.4,
          itemStyle: { color: c.accent, shadowBlur: 25, shadowColor: c.accent, borderWidth: 3 },
          label: { fontSize: 12, fontWeight: 700, color: c.accent },
        },
        zlevel: 3,
      },
      {
        type: 'scatter',
        coordinateSystem: 'geo',
        data: scatterData,
        symbol: arrowSymbol,
        symbolSize: 10,
        symbolRotate: 0,
        silent: false,
        tooltip: { show: false },
        itemStyle: {
          color: c.accent,
          shadowBlur: 8,
          shadowColor: c.accent,
        },
        emphasis: {
          scale: 1.5,
          itemStyle: { color: '#fff', shadowBlur: 15 },
        },
        zlevel: 4,
      },
    ],
  };
};

const getRankingOption = () => {
  const c = getColors();
  const schools = schoolList.value.length ? schoolList.value.slice(0, 10) : ['暂无数据'];
  const baseRate = currentCountyData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rates = schools.map((_, i) => Math.round((baseRate + (12 - i * 1.2) + ((seed + i * 3) % 5 - 2)) * 10) / 10);
  const maxRate = Math.max(...rates);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const p = params[0];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`;
      },
    },
    grid: { left: 10, right: 55, top: 10, bottom: 10, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category', data: schools, inverse: true,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold', interval: 0 },
    },
    series: [{
      name: currentMetric.value.name + '率',
      type: 'bar',
      barGap: '30%',
      barCategoryGap: '40%',
      data: rates.map(v => ({
        value: v,
        itemStyle: {
          color: makeGrad(c.primary, c.secondary),
          borderRadius: [0, 4, 4, 0],
          shadowBlur: isLight.value ? 4 : 8, shadowColor: `${c.primary}44`,
        },
      })),
      barWidth: 10,
      label: { show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
    }],
  };
};

const getAgeGenderOption = () => {
  const c = getColors();
  const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
  const d = {
    grades,
    male: grades.map((_, i) => Math.round(22 + i * 7 + (Math.random() * 4 - 2))),
    female: grades.map((_, i) => Math.round(20 + i * 8 + (Math.random() * 4 - 2)))
  };
  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const i = params[0].dataIndex;
        return `<div style="font-weight:600;margin-bottom:4px">${d.grades[i]}</div>
                <div>■男生：<span style="color:${c.primary};font-weight:bold">${d.male[i]}%</span></div>
                <div>■女生：<span style="color:${c.secondary};font-weight:bold">${d.female[i]}%</span></div>`;
      },
    },
    legend: { data: ['男生', '女生'], textStyle: { color: c.text, fontSize: 11 }, right: 10, top: 2, itemWidth: 12, itemHeight: 12 },
    grid: { left: 40, right: 25, top: 30, bottom: 20, containLabel: true },
    xAxis: {
      type: 'value', min: -100, max: 100, splitNumber: 5,
      axisLine: { show: false }, axisTick: { show: false },
      splitLine: { lineStyle: { color: c.splitLine } },
      axisLabel: { color: c.textDim, fontSize: 10, formatter: (v) => Math.abs(v) + '' },
    },
    yAxis: {
      type: 'category', data: d.grades,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生', type: 'bar', stack: 'total', data: d.male.map((v) => -v), barWidth: 14,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: { show: true, position: 'left', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: (p) => Math.abs(p.value) + '%' },
      },
      {
        name: '女生', type: 'bar', stack: 'total', data: d.female, barWidth: 14,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#c4b5fd' : '#7c3aed'), borderRadius: [0, 4, 4, 0] },
        label: { show: true, position: 'right', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
      },
    ],
  };
};

const getGaugeOption = (value, name, color) => {
  const c = getColors();
  return {
    backgroundColor: 'transparent',
    series: [{
      type: 'gauge', startAngle: 200, endAngle: -20, min: 0, max: 100,
      radius: '92%', center: ['50%', '58%'],
      progress: {
        show: true, width: 10, roundCap: true,
        itemStyle: {
          color: makeGrad(color, color === c.primary ? c.secondary : c.accent),
          shadowBlur: isLight.value ? 6 : 12, shadowColor: `${color}88`,
        },
      },
      axisLine: { lineStyle: { width: 10, color: [[1, c.gaugeTrack]] } },
      pointer: { show: false }, axisTick: { show: false }, splitLine: { show: false },
      axisLabel: { show: false }, anchor: { show: false },
      detail: {
        valueAnimation: true, formatter: '{value}%', color, fontSize: 22, fontWeight: 'bold',
        offsetCenter: [0, '15%'], textShadowBlur: isLight.value ? 0 : 8, textShadowColor: color,
      },
      title: { show: true, offsetCenter: [0, '70%'], color: c.textDim, fontSize: 12 },
      data: [{ value, name }],
    }],
  };
};

const getTrendOption = () => {
  const c = getColors();
  const months = ['1月', '2月', '3月', '4月', '5月', '6月'];
  const baseRate = currentCountyData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rateData = months.map((_, i) => Math.round((baseRate + i * 0.7 + ((seed + i * 3) % 4 - 2)) * 10) / 10);
  const targetData = months.map((_, i) => Math.round((baseRate + 2 - i * 0.3) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { show: false },
    grid: { left: 40, right: 20, top: 10, bottom: 30, containLabel: true },
    xAxis: {
      type: 'category', data: months,
      axisLine: { lineStyle: { color: c.splitLine } }, axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
    },
    yAxis: {
      type: 'value', axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
      splitLine: { lineStyle: { color: c.splitLine } },
    },
    series: [
      {
        name: '实际' + currentMetric.value.name + '率', type: 'line', data: rateData,
        smooth: true, symbol: 'circle', symbolSize: 8,
        lineStyle: { width: 3, color: c.primary, shadowBlur: 8, shadowColor: `${c.primary}55` },
        itemStyle: { color: c.primary, borderColor: c.bg, borderWidth: 2 },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: `${c.primary}55` }, { offset: 1, color: `${c.primary}00` },
          ]),
        },
      },
      {
        name: '目标线', type: 'line', data: targetData,
        smooth: true, symbol: 'none',
        lineStyle: { width: 2, color: c.secondary, type: 'dashed' },
      },
    ],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentCountyData.value.rate;
  const data = [
    { name: '示范区A', initial: Math.round((base + 6 + (seed % 5)) * 10) / 10, final: Math.round((base + 1 - (seed % 3)) * 10) / 10, change: -Math.round((6 + (seed % 3)) * 10) / 10 },
    { name: '示范区B', initial: Math.round((base + 10 + (seed % 4)) * 10) / 10, final: Math.round((base + 2 - (seed % 4)) * 10) / 10, change: -Math.round((8 + (seed % 2)) * 10) / 10 },
    { name: '示范区C', initial: Math.round((base + 4 + (seed % 3)) * 10) / 10, final: Math.round((base + 1 - (seed % 2)) * 10) / 10, change: -Math.round((4 + (seed % 4)) * 10) / 10 },
    { name: '示范区D', initial: Math.round((base + 8 + (seed % 2)) * 10) / 10, final: Math.round((base + 2 - (seed % 3)) * 10) / 10, change: -Math.round((7 + (seed % 2)) * 10) / 10 },
  ];
  const names = data.map(d => d.name);
  const initials = data.map(d => d.initial);
  const finals = data.map(d => d.final);
  const changes = data.map(d => d.change);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        let html = '';
        params.forEach((p) => { html += `<div>${p.marker}${p.seriesName}：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`; });
        return html;
      },
    },
    legend: { show: false },
    grid: { left: 80, right: 60, top: 8, bottom: 8, containLabel: true },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...initials) * 1.2) },
    yAxis: { type: 'category', data: names, axisLine: { show: false }, axisTick: { show: false }, axisLabel: { color: c.text, fontSize: 11 } },
    series: [
      {
        name: '期初', type: 'bar', data: initials, barWidth: 9, barGap: '30%',
        itemStyle: { color: c.initialBar, borderRadius: [0, 3, 3, 0] },
        label: { show: true, position: 'right', color: c.textDim, fontSize: 10, formatter: '{c}%', distance: 4 },
      },
      {
        name: '期末', type: 'bar', data: finals, barWidth: 9,
        itemStyle: {
          color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'),
          borderRadius: [0, 3, 3, 0],
          shadowBlur: isLight.value ? 4 : 8, shadowColor: `${c.primary}55`,
        },
        label: {
          show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold',
          formatter: (p) => {
            const ch = changes[p.dataIndex];
            return `${p.value}%  {ch|${ch > 0 ? '+' : ''}${ch}}`;
          },
          rich: { ch: { color: c.success, fontSize: 9, padding: [0, 0, 0, 4] } },
        },
      },
    ],
  };
};

const renderAllCharts = () => {
  disposeAll();
  requestAnimationFrame(() => {
    nextTick(() => {
      const d = currentCountyData.value;
      const ur = { urban: Math.round(d.rate * 0.9 * 10) / 10, rural: Math.round(d.rate * 1.2 * 10) / 10 };
      initChart(rankingChartRef, 'ranking', getRankingOption(), (params) => {
        if (params?.name) drillToSchool(params.name);
      });
      initChart(ageGenderChartRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, getColors().primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, getColors().secondary));
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToSchool(params.name);
        });
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const cfg = currentCountyConfig.value;
  const urls = [
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}_full.json`,
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}.json`,
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('county', geoJson);
      mapLoaded.value = true;
      mapZoom.value = 1.0;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
  mapLoaded.value = true;
  setTimeout(() => renderAllCharts(), 200);
};

const resizeAll = () => {
  Object.values(chartInstances).forEach(c => { if (c && !c.isDisposed()) c.resize(); });
};

onMounted(() => {
  applySystemTheme();
  observeTheme();
  updateDateTime();
  timer = setInterval(updateDateTime, 1000);
  loadMap();
  renderAllCharts();
  window.addEventListener('resize', resizeAll);
  document.addEventListener('click', handleDropdownClickOutside);
  setTimeout(() => resizeAll(), 500);
  setTimeout(() => resizeAll(), 1500);
});

onBeforeUnmount(() => {
  if (timer) clearInterval(timer);
  window.removeEventListener('resize', resizeAll);
  document.removeEventListener('click', handleDropdownClickOutside);
  if (themeObserver) { themeObserver.disconnect(); themeObserver = null; }
  disposeAll();
});

watch(activeTab, () => setTimeout(() => renderAllCharts(), 100));
watch(isLight, () => setTimeout(() => renderAllCharts(), 100));
watch(() => route.query.tab, (tab) => {
  if (tab && ['vision','oral','mental','weight','bone'].includes(tab)) {
    activeTab.value = tab;
    setTimeout(() => renderAllCharts(), 100);
  }
}, { immediate: true });
watch(() => route.params.code, () => {
  mapLoaded.value = false;
  setTimeout(() => loadMap(), 100);
});