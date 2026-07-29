import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import * as echarts from 'echarts';

const router = useRouter();
const route = useRoute();

const DARK_COLOR = {
  primary: '#38bdf8',
  secondary: '#a78bfa',
  accent: '#f472b6',
  warning: '#fbbf24',
  success: '#34d399',
  danger: '#fb7185',
  male: '#38bdf8',
  female: '#a78bfa',
  text: '#e2e8f0',
  textDim: '#94a3b8',
  bg: 'rgba(10, 22, 50, 0.96)',
  mapArea: 'rgba(15, 30, 65, 0.85)',
  mapBorder: 'rgba(56, 189, 248, 0.75)',
  mapLabel: '#e2e8f0',
  splitLine: 'rgba(56, 189, 248, 0.12)',
  gaugeTrack: 'rgba(56, 189, 248, 0.12)',
  initialBar: '#475569',
  mapColors: ['#0891b2', '#38bdf8', '#a78bfa', '#f472b6'],
};

const LIGHT_COLOR = {
  primary: '#0891b2',
  secondary: '#7c3aed',
  accent: '#db2777',
  warning: '#d97706',
  success: '#059669',
  danger: '#dc2626',
  male: '#0891b2',
  female: '#7c3aed',
  text: '#1e293b',
  textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)',
  mapArea: '#f1f5f9',
  mapBorder: 'rgba(8, 145, 178, 0.35)',
  mapLabel: '#1e293b',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)',
  initialBar: '#cbd5e1',
  mapColors: ['#67e8f9', '#0891b2', '#7c3aed', '#db2777'],
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 },
    { offset: 1, color: c2 },
  ]);

const PROVINCE_MAP_CONFIG = {
  '110000': { name: '北京市', short: '北京', center: [116.4, 39.9], zoom: 1.6 },
  '120000': { name: '天津市', short: '天津', center: [117.2, 39.1], zoom: 1.8 },
  '130000': { name: '河北省', short: '河北', center: [114.5, 38.0], zoom: 1.8 },
  '140000': { name: '山西省', short: '山西', center: [112.5, 37.8], zoom: 1.8 },
  '150000': { name: '内蒙古自治区', short: '内蒙古', center: [111.7, 40.8], zoom: 1.4 },
  '210000': { name: '辽宁省', short: '辽宁', center: [123.4, 41.8], zoom: 1.7 },
  '220000': { name: '吉林省', short: '吉林', center: [125.3, 43.9], zoom: 1.6 },
  '230000': { name: '黑龙江省', short: '黑龙江', center: [126.6, 45.7], zoom: 1.5 },
  '310000': { name: '上海市', short: '上海', center: [121.5, 31.2], zoom: 2.0 },
  '320000': { name: '江苏省', short: '江苏', center: [118.8, 32.0], zoom: 1.8 },
  '330000': { name: '浙江省', short: '浙江', center: [120.2, 29.2], zoom: 1.8 },
  '340000': { name: '安徽省', short: '安徽', center: [117.3, 31.8], zoom: 1.8 },
  '350000': { name: '福建省', short: '福建', center: [119.3, 26.1], zoom: 1.8 },
  '360000': { name: '江西省', short: '江西', center: [115.9, 28.7], zoom: 1.8 },
  '370000': { name: '山东省', short: '山东', center: [118.0, 36.5], zoom: 1.8 },
  '410000': { name: '河南省', short: '河南', center: [113.6, 34.8], zoom: 1.8 },
  '420000': { name: '湖北省', short: '湖北', center: [112.3, 31.0], zoom: 1.8 },
  '430000': { name: '湖南省', short: '湖南', center: [112.0, 27.6], zoom: 1.8 },
  '440000': { name: '广东省', short: '广东', center: [113.5, 23.8], zoom: 1.8 },
  '450000': { name: '广西壮族自治区', short: '广西', center: [108.3, 22.8], zoom: 1.7 },
  '460000': { name: '海南省', short: '海南', center: [110.3, 20.0], zoom: 1.8 },
  '500000': { name: '重庆市', short: '重庆', center: [106.5, 29.5], zoom: 1.9 },
  '510000': { name: '四川省', short: '四川', center: [104.0, 30.6], zoom: 1.6 },
  '520000': { name: '贵州省', short: '贵州', center: [106.7, 26.6], zoom: 1.8 },
  '530000': { name: '云南省', short: '云南', center: [102.7, 25.0], zoom: 1.7 },
  '540000': { name: '西藏自治区', short: '西藏', center: [91.1, 29.7], zoom: 1.2 },
  '610000': { name: '陕西省', short: '陕西', center: [108.9, 34.3], zoom: 1.8 },
  '620000': { name: '甘肃省', short: '甘肃', center: [103.8, 36.1], zoom: 1.4 },
  '630000': { name: '青海省', short: '青海', center: [101.8, 36.6], zoom: 1.4 },
  '640000': { name: '宁夏回族自治区', short: '宁夏', center: [106.3, 38.5], zoom: 1.7 },
  '650000': { name: '新疆维吾尔自治区', short: '新疆', center: [87.6, 43.8], zoom: 1.2 },
};

const CITY_CODES_MAP = {
  '110000': { '东城区': '110101', '西城区': '110102', '朝阳区': '110105', '海淀区': '110108', '丰台区': '110106', '石景山区': '110107', '通州区': '110112', '昌平区': '110114', '顺义区': '110113', '大兴区': '110115', '和平区': '110116', '东城区': '110101' },
  '120000': { '和平区': '120101', '河东区': '120102', '河西区': '120103', '南开区': '120104', '河北区': '120105', '红桥区': '120106', '东丽区': '120110', '西青区': '120111', '津南区': '120112', '北辰区': '120113' },
  '130000': { '石家庄市': '130100', '唐山市': '130200', '秦皇岛市': '130300', '邯郸市': '130400', '邢台市': '130500', '保定市': '130600', '张家口市': '130700', '承德市': '130800', '沧州市': '130900', '廊坊市': '131000', '衡水市': '131100' },
  '140000': { '太原市': '140100', '大同市': '140200', '阳泉市': '140300', '长治市': '140400', '晋城市': '140500', '朔州市': '140600', '晋中市': '140700', '运城市': '140800', '忻州市': '140900', '临汾市': '141000', '吕梁市': '141100' },
  '150000': { '呼和浩特市': '150100', '包头市': '150200', '乌海市': '150300', '赤峰市': '150400', '通辽市': '150500', '鄂尔多斯市': '150600', '呼伦贝尔市': '150700', '巴彦淖尔市': '150800', '乌兰察布市': '150900', '兴安盟': '152200' },
  '210000': { '沈阳市': '210100', '大连市': '210200', '鞍山市': '210300', '抚顺市': '210400', '本溪市': '210500', '丹东市': '210600', '锦州市': '210700', '营口市': '210800', '阜新市': '210900', '辽阳市': '211000', '盘锦市': '211100', '铁岭市': '211200', '朝阳市': '211300', '葫芦岛市': '211400' },
  '220000': { '长春市': '220100', '吉林市': '220200', '四平市': '220300', '辽源市': '220400', '通化市': '220500', '白山市': '220600', '松原市': '220700', '白城市': '220800' },
  '230000': { '哈尔滨市': '230100', '齐齐哈尔市': '230200', '鸡西市': '230300', '鹤岗市': '230400', '双鸭山市': '230500', '大庆市': '230600', '伊春市': '230700', '佳木斯市': '230800', '七台河市': '230900', '牡丹江市': '231000' },
  '310000': { '黄浦区': '310101', '徐汇区': '310104', '长宁区': '310105', '静安区': '310106', '普陀区': '310107', '虹口区': '310109', '杨浦区': '310110', '浦东新区': '310115', '闵行区': '310112', '宝山区': '310113', '嘉定区': '310114', '金山区': '310116', '松江区': '310117', '青浦区': '310118', '奉贤区': '310120', '崇明区': '310151' },
  '320000': { '南京市': '320100', '无锡市': '320200', '徐州市': '320300', '常州市': '320400', '苏州市': '320500', '南通市': '320600', '连云港市': '320700', '淮安市': '320800', '盐城市': '320900', '扬州市': '321000', '镇江市': '321100', '泰州市': '321200', '宿迁市': '321300' },
  '330000': { '杭州市': '330100', '宁波市': '330200', '温州市': '330300', '嘉兴市': '330400', '湖州市': '330500', '绍兴市': '330600', '金华市': '330700', '衢州市': '330800', '舟山市': '330900', '台州市': '331000', '丽水市': '331100' },
  '340000': { '合肥市': '340100', '芜湖市': '340200', '蚌埠市': '340300', '淮南市': '340400', '马鞍山市': '340500', '淮北市': '340600', '铜陵市': '340700', '安庆市': '340800', '黄山市': '341000', '滁州市': '341100', '阜阳市': '341200', '宿州市': '341300', '六安市': '341500' },
  '350000': { '福州市': '350100', '厦门市': '350200', '莆田市': '350300', '三明市': '350400', '泉州市': '350500', '漳州市': '350600', '南平市': '350700', '龙岩市': '350800', '宁德市': '350900' },
  '360000': { '南昌市': '360100', '景德镇市': '360200', '萍乡市': '360300', '九江市': '360400', '新余市': '360500', '鹰潭市': '360600', '赣州市': '360700', '吉安市': '360800', '宜春市': '360900', '抚州市': '361000', '上饶市': '361100' },
  '370000': { '济南市': '370100', '青岛市': '370200', '淄博市': '370300', '枣庄市': '370400', '东营市': '370500', '烟台市': '370600', '潍坊市': '370700', '济宁市': '370800', '泰安市': '370900', '威海市': '371000', '日照市': '371100', '临沂市': '371300', '德州市': '371400', '聊城市': '371500', '滨州市': '371600', '菏泽市': '371700' },
  '410000': { '郑州市': '410100', '开封市': '410200', '洛阳市': '410300', '平顶山市': '410400', '安阳市': '410500', '鹤壁市': '410600', '新乡市': '410700', '焦作市': '410800', '濮阳市': '410900', '许昌市': '411000', '漯河市': '411100', '三门峡市': '411200', '南阳市': '411300', '商丘市': '411400', '信阳市': '411500', '周口市': '411600', '驻马店市': '411700' },
  '420000': { '武汉市': '420100', '黄石市': '420200', '十堰市': '420300', '宜昌市': '420500', '襄阳市': '420600', '鄂州市': '420700', '荆门市': '420800', '孝感市': '420900', '荆州市': '421000', '黄冈市': '421100', '咸宁市': '421200', '随州市': '421300' },
  '430000': { '长沙市': '430100', '株洲市': '430200', '湘潭市': '430300', '衡阳市': '430400', '邵阳市': '430500', '岳阳市': '430600', '常德市': '430700', '张家界市': '430800', '益阳市': '430900', '郴州市': '431000', '永州市': '431100', '怀化市': '431200', '娄底市': '431300' },
  '440000': { '广州市': '440100', '深圳市': '440300', '珠海市': '440400', '汕头市': '440500', '佛山市': '440600', '韶关市': '440200', '湛江市': '440800', '肇庆市': '441200', '江门市': '440700', '茂名市': '440900', '惠州市': '441300', '梅州市': '441400', '汕尾市': '441500', '河源市': '441600', '阳江市': '441700', '清远市': '441800', '东莞市': '441900', '中山市': '442000', '潮州市': '445100', '揭阳市': '445200' },
  '450000': { '南宁市': '450100', '柳州市': '450200', '桂林市': '450300', '梧州市': '450400', '北海市': '450500', '防城港市': '450600', '钦州市': '450700', '贵港市': '450800', '玉林市': '450900', '百色市': '451000', '贺州市': '451100', '河池市': '451200', '来宾市': '451300', '崇左市': '451400' },
  '460000': { '海口市': '460100', '三亚市': '460200', '三沙市': '460300', '儋州市': '460400' },
  '500000': { '渝中区': '500103', '江北区': '500105', '沙坪坝区': '500106', '九龙坡区': '500107', '南岸区': '500108', '北碚区': '500109', '万州区': '500101', '涪陵区': '500102', '渝北区': '500112', '巴南区': '500113' },
  '510000': { '成都市': '510100', '自贡市': '510300', '攀枝花市': '510400', '泸州市': '510500', '德阳市': '510600', '绵阳市': '510700', '广元市': '510800', '遂宁市': '510900', '内江市': '511000', '乐山市': '511100', '南充市': '511300', '眉山市': '511400' },
  '520000': { '贵阳市': '520100', '六盘水市': '520200', '遵义市': '520300', '安顺市': '520400', '毕节市': '520500', '铜仁市': '520600' },
  '530000': { '昆明市': '530100', '曲靖市': '530300', '玉溪市': '530400', '保山市': '530500', '昭通市': '530600', '丽江市': '530700', '普洱市': '530800', '临沧市': '530900' },
  '540000': { '拉萨市': '540100', '日喀则市': '540200', '昌都市': '540300', '林芝市': '540400', '山南市': '540500', '那曲市': '540600' },
  '610000': { '西安市': '610100', '铜川市': '610200', '宝鸡市': '610300', '咸阳市': '610400', '渭南市': '610500', '延安市': '610600', '汉中市': '610700', '榆林市': '610800', '安康市': '610900', '商洛市': '611000' },
  '620000': { '兰州市': '620100', '嘉峪关市': '620200', '金昌市': '620300', '白银市': '620400', '天水市': '620500', '武威市': '620600', '张掖市': '620700', '平凉市': '620800', '酒泉市': '620900', '庆阳市': '621000', '定西市': '621100', '陇南市': '621200' },
  '630000': { '西宁市': '630100', '海东市': '630200' },
  '640000': { '银川市': '640100', '石嘴山市': '640200', '吴忠市': '640300', '固原市': '640400', '中卫市': '640500' },
  '650000': { '乌鲁木齐市': '650100', '克拉玛依市': '650200', '吐鲁番市': '650400', '哈密市': '650500' },
};

const currentCityCodes = computed(() => {
  return CITY_CODES_MAP[routeCode.value] || {};
});

const drillToCity = (name) => {
  const codes = currentCityCodes.value;
  let code = codes[name];
  if (!code) {
    const suffixes = ['市', '区', '县', '盟', '自治州', '地区'];
    for (const s of suffixes) {
      const stripped = name.replace(s, '');
      if (codes[stripped]) { code = codes[stripped]; break; }
    }
  }
  if (code) router.push({ path: `/vision/city/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到城市编码:', name);
};

const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const mapLoaded = ref(false);
const mapZoom = ref(1.8);
let timer = null;
let themeObserver = null;

const routeCode = computed(() => String(route.params.code || '320000'));
const currentProvinceConfig = computed(() => {
  return PROVINCE_MAP_CONFIG[routeCode.value] || PROVINCE_MAP_CONFIG['320000'];
});
const currentProvince = computed(() => currentProvinceConfig.value.short);
const currentProvinceCenter = computed(() => currentProvinceConfig.value.center);

const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

const metricConfig = {
  vision: { name: '近视', baseRate: 58, trend: 2.5, maleFactor: 0.95, femaleFactor: 1.05, primaryFactor: 0.6, juniorFactor: 1.0, seniorFactor: 1.25 },
  oral: { name: '龋齿', baseRate: 42, trend: -1.2, maleFactor: 0.96, femaleFactor: 1.05, primaryFactor: 1.24, juniorFactor: 0.9, seniorFactor: 0.67 },
  mental: { name: '心理预警', baseRate: 18, trend: -0.8, maleFactor: 0.92, femaleFactor: 1.1, primaryFactor: 0.67, juniorFactor: 1.22, seniorFactor: 1.56 },
  weight: { name: '超重/肥胖', baseRate: 24, trend: 1.5, maleFactor: 1.12, femaleFactor: 0.88, primaryFactor: 0.63, juniorFactor: 1.17, seniorFactor: 1.33 },
  bone: { name: '骨密度偏低', baseRate: 16, trend: -0.5, maleFactor: 0.91, femaleFactor: 1.11, primaryFactor: 0.75, juniorFactor: 1.13, seniorFactor: 1.38 }
};

const currentMetric = computed(() => metricConfig[activeTab.value]);

const cityList = ref([]);

const districtList = computed(() => cityList.value.slice(0, 6));

const currentFilter = reactive({
  year: '2026',
  district: ''
});

const computeProvinceMetricData = () => {
  const m = metricConfig[activeTab.value];
  const cfg = currentProvinceConfig.value;
  const code = routeCode.value;
  const seed = parseInt(code.slice(-2), 10) || 13;
  const rate = Math.round((m.baseRate + (seed % 15 - 7)) * 10) / 10;
  const students = Math.round(800 + seed * 3 + cfg.center[0] % 50);
  const cities = cityList.value.length || 11;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const primary = Math.round(rate * m.primaryFactor * 10) / 10;
  const junior = Math.round(rate * m.juniorFactor * 10) / 10;
  const senior = Math.round(rate * m.seniorFactor * 10) / 10;
  const topCity = cityList.value[0] || '最高';
  const topRate = Math.round((rate + 5 + (seed % 8)) * 10) / 10;
  return { rate, students, cities, trend: m.trend, maleRate, femaleRate, primary, junior, senior, topCity, topRate };
};

const currentProvinceData = computed(() => computeProvinceMetricData());

const rankingChartRef = ref(null);
const ageGenderChartRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const trendChartRef = ref(null);
const interventionChartRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};

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

const handleSearch = () => {
  renderAllCharts();
};

const handleReset = () => {
  currentFilter.year = '2026';
  currentFilter.district = '';
};

const goBack = () => {
  router.push({ path: '/vision/national', query: { tab: activeTab.value } });
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

const getProvinceMapData = () => {
  const cities = cityList.value;
  if (cities.length === 0) return [];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  return cities.map((name, i) => ({
    name: name,
    value: Math.round((baseRate + ((seed + i * 7) % 20 - 10)) * 10) / 10
  }));
};

const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();
  const data = getProvinceMapData();
  if (data.length === 0) return {};
  const values = data.map(d => d.value);
  const minV = Math.min(...values);
  const maxV = Math.max(...values);
  const cfg = currentProvinceConfig.value;

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        const canDrill = !!currentCityCodes.value[p.name];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value || 0}%</span></div>${canDrill ? '<div style="font-size:11px;color:var(--text-dim);margin-top:4px">👆 点击查看市级详情</div>' : ''}`;
      },
    },
    visualMap: { show: false, min: minV, max: maxV, inRange: { color: c.mapColors } },
    series: [{
      type: 'map',
      map: 'province',
      roam: true,
      selectedMode: false,
      zoom: cfg.zoom,
      center: cfg.center,
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
          borderColor: isLight.value ? 'rgba(8,145,178,0.3)' : c.primary, borderWidth: 2, shadowBlur: 20, shadowColor: `${c.primary}aa`,
        },
      },
      data,
    }],
  };
};

const getRankingOption = () => {
  const c = getColors();
  const cities = cityList.value.length ? cityList.value.slice(0, 10) : ['暂无数据'];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rates = cities.map((_, i) => Math.round((baseRate + (15 - i * 1.5) + ((seed + i * 3) % 6 - 3)) * 10) / 10);
  const maxRate = Math.max(...rates);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const p = params[0];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div><div style="font-size:11px;color:var(--text-dim);margin-top:4px">👆 点击查看市级详情</div>`;
      },
    },
    grid: { left: 10, right: 50, top: 5, bottom: 5, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category',
      data: cities,
      inverse: true,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [{
      name: currentMetric.value.name + '率',
      type: 'bar',
      data: rates.map(v => ({
        value: v,
        itemStyle: {
          color: makeGrad(c.primary, c.secondary),
          borderRadius: [0, 4, 4, 0],
          shadowBlur: isLight.value ? 4 : 8,
          shadowColor: `${c.primary}44`,
        },
      })),
      barWidth: 12,
      label: {
        show: true,
        position: 'right',
        color: c.primary,
        fontSize: 10,
        fontWeight: 'bold',
        formatter: '{c}%',
      },
    }],
  };
};

const getAgeGenderOption = () => {
  const c = getColors();
  const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
  const d = {
    grades,
    male: grades.map((_, i) => Math.round(25 + i * 8 + (Math.random() * 4 - 2))),
    female: grades.map((_, i) => Math.round(22 + i * 9 + (Math.random() * 4 - 2)))
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
    legend: {
      data: ['男生', '女生'],
      textStyle: { color: c.text, fontSize: 11 },
      right: 10,
      top: 2,
      itemWidth: 12,
      itemHeight: 12,
    },
    grid: { left: 40, right: 25, top: 30, bottom: 20 },
    xAxis: {
      type: 'value',
      min: -100,
      max: 100,
      splitNumber: 5,
      axisLine: { show: false },
      axisTick: { show: false },
      splitLine: { lineStyle: { color: c.splitLine } },
      axisLabel: { color: c.textDim, fontSize: 10, formatter: (v) => Math.abs(v) + '' },
    },
    yAxis: {
      type: 'category',
      data: d.grades,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生',
        type: 'bar',
        stack: 'total',
        data: d.male.map((v) => -v),
        barWidth: 14,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: {
          show: true,
          position: 'left',
          color: c.text,
          fontSize: 10,
          fontWeight: 'bold',
          formatter: (p) => Math.abs(p.value) + '%',
        },
      },
      {
        name: '女生',
        type: 'bar',
        stack: 'total',
        data: d.female,
        barWidth: 14,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#c4b5fd' : '#7c3aed'), borderRadius: [0, 4, 4, 0] },
        label: {
          show: true,
          position: 'right',
          color: c.text,
          fontSize: 10,
          fontWeight: 'bold',
          formatter: '{c}%',
        },
      },
    ],
  };
};

const getGaugeOption = (value, name, color) => {
  const c = getColors();
  return {
    backgroundColor: 'transparent',
    series: [{
      type: 'gauge',
      startAngle: 200,
      endAngle: -20,
      min: 0,
      max: 100,
      radius: '92%',
      center: ['50%', '58%'],
      progress: {
        show: true,
        width: 10,
        roundCap: true,
        itemStyle: {
          color: makeGrad(color, color === c.primary ? c.secondary : c.accent),
          shadowBlur: isLight.value ? 6 : 12,
          shadowColor: `${color}88`,
        },
      },
      axisLine: { lineStyle: { width: 10, color: [[1, c.gaugeTrack]] } },
      pointer: { show: false },
      axisTick: { show: false },
      splitLine: { show: false },
      axisLabel: { show: false },
      anchor: { show: false },
      detail: {
        valueAnimation: true,
        formatter: '{value}%',
        color,
        fontSize: 22,
        fontWeight: 'bold',
        offsetCenter: [0, '15%'],
        textShadowBlur: isLight.value ? 0 : 8,
        textShadowColor: color,
      },
      title: { show: true, offsetCenter: [0, '70%'], color: c.textDim, fontSize: 12 },
      data: [{ value, name }],
    }],
  };
};

const getTrendOption = () => {
  const c = getColors();
  const months = ['1月', '2月', '3月', '4月', '5月', '6月'];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rateData = months.map((_, i) => Math.round((baseRate + i * 0.8 + ((seed + i * 3) % 4 - 2)) * 10) / 10);
  const targetData = months.map((_, i) => Math.round((baseRate + 2 - i * 0.3) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { show: false },
    grid: { left: 40, right: 20, top: 10, bottom: 30 },
    xAxis: {
      type: 'category',
      data: months,
      axisLine: { lineStyle: { color: c.splitLine } },
      axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
    },
    yAxis: {
      type: 'value',
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
      splitLine: { lineStyle: { color: c.splitLine } },
    },
    series: [
      {
        name: '实际' + currentMetric.value.name + '率',
        type: 'line',
        data: rateData,
        smooth: true,
        symbol: 'circle',
        symbolSize: 8,
        lineStyle: { width: 3, color: c.primary, shadowBlur: 8, shadowColor: `${c.primary}55` },
        itemStyle: { color: c.primary, borderColor: c.bg, borderWidth: 2 },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: `${c.primary}55` },
            { offset: 1, color: `${c.primary}00` },
          ]),
        },
      },
      {
        name: '目标线',
        type: 'line',
        data: targetData,
        smooth: true,
        symbol: 'none',
        lineStyle: { width: 2, color: c.secondary, type: 'dashed' },
      },
    ],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentProvinceData.value.rate;
  const data = [
    { name: '示范区A', initial: Math.round((base + 8 + (seed % 5)) * 10) / 10, final: Math.round((base + 1 - (seed % 3)) * 10) / 10, change: -Math.round((7 + (seed % 3)) * 10) / 10 },
    { name: '示范区B', initial: Math.round((base + 12 + (seed % 4)) * 10) / 10, final: Math.round((base + 3 - (seed % 4)) * 10) / 10, change: -Math.round((9 + (seed % 2)) * 10) / 10 },
    { name: '示范区C', initial: Math.round((base + 5 + (seed % 3)) * 10) / 10, final: Math.round((base + 1 - (seed % 2)) * 10) / 10, change: -Math.round((5 + (seed % 4)) * 10) / 10 },
    { name: '示范区D', initial: Math.round((base + 10 + (seed % 2)) * 10) / 10, final: Math.round((base + 2 - (seed % 3)) * 10) / 10, change: -Math.round((8 + (seed % 2)) * 10) / 10 },
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
        params.forEach((p) => {
          html += `<div>${p.marker}${p.seriesName}：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`;
        });
        return html;
      },
    },
    legend: { show: false },
    grid: { left: 80, right: 60, top: 8, bottom: 8 },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...initials) * 1.2) },
    yAxis: {
      type: 'category',
      data: names,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11 },
    },
    series: [
      {
        name: '期初',
        type: 'bar',
        data: initials,
        barWidth: 9,
        barGap: '30%',
        itemStyle: { color: c.initialBar, borderRadius: [0, 3, 3, 0] },
        label: { show: true, position: 'right', color: c.textDim, fontSize: 10, formatter: '{c}%', distance: 4 },
      },
      {
        name: '期末',
        type: 'bar',
        data: finals,
        barWidth: 9,
        itemStyle: {
          color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'),
          borderRadius: [0, 3, 3, 0],
          shadowBlur: isLight.value ? 4 : 8,
          shadowColor: `${c.primary}55`,
        },
        label: {
          show: true,
          position: 'right',
          color: c.primary,
          fontSize: 10,
          fontWeight: 'bold',
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
      const d = currentProvinceData.value;
      const ur = { urban: Math.round(d.rate * 0.9 * 10) / 10, rural: Math.round(d.rate * 1.2 * 10) / 10 };
      initChart(rankingChartRef, 'ranking', getRankingOption(), (params) => {
        if (params?.name) drillToCity(params.name);
      });
      initChart(ageGenderChartRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, getColors().primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, getColors().secondary));
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToCity(params.name);
        });
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const cfg = currentProvinceConfig.value;
  const urls = [
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}_full.json`,
    `https://fastly.jsdelivr.net/npm/echarts@4.9.0/map/json/province/${cfg.short}.json`,
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}.json`,
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('province', geoJson);
      const features = geoJson.features || [];
      if (features.length > 0) {
        const list = [];
        features.forEach(f => {
          const props = f.properties || {};
          const fullName = props.name || props.NAME || props.NL_NAME_1 || '';
          if (fullName && fullName.length > 0 && fullName.length < 15) {
            list.push(fullName);
          }
        });
        if (list.length >= 3) {
          cityList.value = list.slice(0, 30);
        } else {
          cityList.value = [];
        }
      } else {
        cityList.value = [];
      }
      mapLoaded.value = true;
      mapZoom.value = cfg.zoom;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
  cityList.value = [];
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
  setTimeout(() => resizeAll(), 500);
  setTimeout(() => resizeAll(), 1500);
});

onBeforeUnmount(() => {
  if (timer) clearInterval(timer);
  window.removeEventListener('resize', resizeAll);
  if (themeObserver) { themeObserver.disconnect(); themeObserver = null; }
  disposeAll();
});

watch(activeTab, () => setTimeout(() => renderAllCharts(), 100));
watch(isLight, () => setTimeout(() => renderAllCharts(), 100));
watch(() => route.query.tab, (tab) => {
  if (tab && typeof tab === 'string' && ['vision','oral','mental','weight','bone'].includes(tab)) {
    activeTab.value = tab;
    setTimeout(() => renderAllCharts(), 100);
  }
}, { immediate: true });
watch(() => route.params.code, () => {
  mapLoaded.value = false;
  cityList.value = [];
  setTimeout(() => loadMap(), 100);
});