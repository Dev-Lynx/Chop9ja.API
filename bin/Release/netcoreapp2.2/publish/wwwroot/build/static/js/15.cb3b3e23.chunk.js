(window.webpackJsonp=window.webpackJsonp||[]).push([[15],{145:function(e,t,a){"use strict";a.d(t,"c",function(){return c}),a.d(t,"d",function(){return s}),a.d(t,"f",function(){return d}),a.d(t,"b",function(){return u}),a.d(t,"e",function(){return m}),a.d(t,"a",function(){return f});var n=a(1),r=a(129),o=a(22),i={"1/2":"50%","1/4":"25%","2/4":"50%","3/4":"75%","1/3":"33.33%","2/3":"66.66%"},l=Object(n.css)(["width:",";max-width:",";overflow:hidden;"],function(e){return i[e.size]||e.theme.global.size[e.size]},function(e){return i[e.size]||e.theme.global.size[e.size]}),c=n.default.td.withConfig({displayName:"StyledTable__StyledTableCell",componentId:"sc-1m3u5g-0"})(["margin:0;padding:0;font-weight:inherit;text-align:inherit;height:100%;"," "," ",""],function(e){return e.size&&l},function(e){return e.verticalAlign&&"vertical-align: "+e.verticalAlign+";"},function(e){return e.tableContextTheme&&e.tableContextTheme.extend});c.defaultProps={},Object.setPrototypeOf(c.defaultProps,o.a);var s=n.default.caption.withConfig({displayName:"StyledTable__StyledTableDataCaption",componentId:"sc-1m3u5g-1"})(["display:none;"]);s.defaultProps={},Object.setPrototypeOf(s.defaultProps,o.a);var d=n.default.tr.withConfig({displayName:"StyledTable__StyledTableRow",componentId:"sc-1m3u5g-2"})(["height:100%;"]);d.defaultProps={},Object.setPrototypeOf(d.defaultProps,o.a);var u=n.default.tbody.withConfig({displayName:"StyledTable__StyledTableBody",componentId:"sc-1m3u5g-3"})([""]);u.defaultProps={},Object.setPrototypeOf(u.defaultProps,o.a);var m=n.default.thead.withConfig({displayName:"StyledTable__StyledTableHeader",componentId:"sc-1m3u5g-4"})([""]);m.defaultProps={},Object.setPrototypeOf(m.defaultProps,o.a);var p=n.default.tfoot.withConfig({displayName:"StyledTable__StyledTableFooter",componentId:"sc-1m3u5g-5"})([""]);p.defaultProps={},Object.setPrototypeOf(p.defaultProps,o.a);var f=n.default.table.withConfig({displayName:"StyledTable",componentId:"sc-1m3u5g-6"})(["border-spacing:0;border-collapse:collapse;width:inherit;@media all and (min--moz-device-pixel-ratio:0){table-layout:fixed;}"," ",";"],r.g,function(e){return e.theme.table&&e.theme.table.extend});f.defaultProps={},Object.setPrototypeOf(f.defaultProps,o.a)},146:function(e,t,a){"use strict";var n=a(24),r=a(312),o=a(309),i=a(313),l=a(286),c=a(287),s=a(288),d=a(0),u=a.n(d),m=a(304),p=a(1);a(148);function f(){var e=Object(n.a)(["\n\twidth: 100%;\n\tpadding: 10px;\n\tz-index: 99999;\n\tposition: fixed;\n\tdisplay: none;\n\ttop: 0;\n\t@media (min-width: 900px) {\n\t\twidth: 500px\n\t}\n"]);return f=function(){return e},e}var g=Object(p.default)(r.a)(f()),h={error:l.a,success:c.a,warning:s.a};t.a=function(e){var t=e.show,a=void 0!==t&&t,n=e.message,r=void 0===n?"Okay now":n,l=e.variant,c=void 0===l?"success":l,s=e.onClose,p=e.duration,f=void 0===p?5e3:p,v=h[c];return Object(d.useEffect)(function(){a&&setTimeout(function(){s(null)},f)},[a]),u.a.createElement(m.a,{in:a,delay:500,classNames:"SnackBar",timeout:4500},u.a.createElement(g,{elevation:"large",background:c,align:"center",justify:"evenly",direction:"row"},u.a.createElement(o.a,{plain:!0,icon:u.a.createElement(v,{color:"white"}),onClick:s,color:"white"}),u.a.createElement(i.a,null,r)))}},147:function(e,t,a){"use strict";a.d(t,"b",function(){return o}),a.d(t,"c",function(){return i}),a.d(t,"a",function(){return l});var n=a(140),r=a.n(n),o={email:[{regexp:/^[\w\-_.]+$/,placeholder:"email"},{fixed:"@"},{regexp:/^[\w]+$/,placeholder:"example"},{fixed:"."},{regexp:/^[\w]+$/,placeholder:"com"}],phone:[{fixed:"+234 "},{length:3,regexp:/^(?!0)[0-9]{1,3}$/},{fixed:" "},{length:3,regexp:/^[0-9]{1,3}$/},{fixed:" "},{length:4,regexp:/^[0-9]{1,4}$/}],date:[{length:[1,2],regexp:/^[1-2][0-9]$|^3[0-1]$|^0?[1-9]$|^0$/,placeholder:"dd"},{fixed:"/"},{length:[1,2],regexp:/^1[0,1-2]$|^0?[1-9]$|^0$/,placeholder:"mm"},{fixed:"/"},{length:4,regexp:/^[1-2]$|^19$|^20$|^19[0-9]$|^20[0-9]$|^19[0-9][0-9]$|^20[0-9][0-9]$/,placeholder:"yyyy"}]},i={email:/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,phone:/^[0-9]{11}$/,numbers:/^[0-9]+$/,globalSpace:/\s/g},l=[{name:"Today",range:function(){return{start:r()().startOf("day").toDate(),end:r()().endOf("day").toDate()}}},{name:"Yesterday",range:function(){var e=r()().subtract(1,"days");return{start:e.startOf("day").toDate(),end:e.endOf("day").toDate()}}},{name:"This week",range:function(){var e=r()();return{start:e.startOf("week").toDate(),end:e.endOf("week").toDate()}}},{name:"Last week",range:function(){var e=r()().subtract(1,"weeks");return{start:e.startOf("week").toDate(),end:e.endOf("week").toDate()}}},{name:"This month",range:function(){var e=r()();return{start:e.startOf("month").toDate(),end:e.endOf("month").toDate()}}},{name:"Last Month",range:function(){var e=r()().subtract(1,"months");return{start:e.startOf("month").toDate(),end:e.endOf("month").toDate()}}},{name:"Last 3 months",range:function(){return{start:r()().subtract(3,"months").startOf("month").toDate(),end:r()().endOf("month").toDate()}}},{name:"This year",range:function(){return{start:r()().startOf("year").toDate(),end:r()().endOf("year").toDate()}}},{name:"Last year",range:function(){var e=r()().subtract(1,"years");return{start:e.startOf("year").toDate(),end:e.endOf("year").toDate()}}},{name:"All time",range:function(){return{start:r()("2000","YYYY").startOf("year").toDate(),end:r()().endOf("year").toDate()}}}]},148:function(e,t,a){},153:function(e,t,a){"use strict";a.d(t,"a",function(){return r});var n=a(0),r=a.n(n).a.createContext(void 0)},157:function(e){e.exports=[{id:"1",name:"Access Bank",knownAs:"Access Bank",code:"044",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535417/Banks/AccessBank.png",isAvailable:!0},{id:"2",name:"Citibank",knownAs:"Citi Bank",code:"023",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535451/Banks/CitiBank.png",isAvailable:!0},{id:"3",name:"Diamond Bank",knownAs:"Diamond Bank",code:"063",isAvailable:!1},{id:"4",name:"Dynamic Standard Bank",knownAs:"Dynamic Standard Bank",code:"",isAvailable:!1},{id:"5",name:"Ecobank Nigeria",knownAs:"EcoBank",code:"050",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535461/Banks/EcoBank.png",isAvailable:!0},{id:"6",name:"Fidelity Bank Nigeria",knownAs:"Fidelity Bank",code:"070",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535465/Banks/FidelityBank.png",isAvailable:!0},{id:"7",name:"First Bank of Nigeria",knownAs:"First Bank",code:"011",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535521/Banks/FirstBank.png",isAvailable:!0},{id:"8",name:"First City Monument Bank",knownAs:"FCMB",code:"214",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535483/Banks/FCMB.png",isAvailable:!0},{id:"9",name:"Guaranty Trust Bank",knownAs:"GT Bank",code:"058",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535515/Banks/GTBank.png",isAvailable:!0},{id:"10",name:"Heritage Bank Plc",knownAs:"Heritage Bank",code:"030",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535516/Banks/HeritageBank.png",isAvailable:!0},{id:"11",name:"Jaiz Bank",knownAs:"Jaiz Bank",code:"301",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535518/Banks/JaizBank.png",isAvailable:!0},{id:"12",name:"Keystone Bank Limited",knownAs:"Keystone Bank",code:"082",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535520/Banks/KeystoneBank.png",isAvailable:!0},{id:"13",name:"Providus Bank Plc",knownAs:"Providus Bank",code:"101",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535526/Banks/ProvidusBank.png",isAvailable:!0},{id:"14",name:"Polaris Bank",knownAs:"Polaris Bank",code:"076",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535521/Banks/PolarisBank.png",isAvailable:!0},{id:"15",name:"Stanbic IBTC Bank Nigeria Limited",knownAs:"Stanbic IBTC Bank",code:"221",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535521/Banks/StanbicBank.png",isAvailable:!0},{id:"16",name:"Standard Chartered Bank",knownAs:"Standard Chartered Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535526/Banks/StandardCharteredBank.png",code:"068",isAvailable:!0},{id:"17",name:"Sterling Bank",knownAs:"Sterling Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535523/Banks/Sterling-Bank.png",code:"232",isAvailable:!0},{id:"18",name:"Suntrust Bank Nigeria Limited",knownAs:"Suntrust Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535524/Banks/SunTrustBank.png",code:"100",isAvailable:!0},{id:"19",name:"Union Bank of Nigeria",knownAs:"Union Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535531/Banks/UnionBank.png",code:"032",isAvailable:!0},{id:"20",name:"United Bank for Africa",knownAs:"UBA",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535526/Banks/UBA.png",code:"033",isAvailable:!0},{id:"21",name:"Unity Bank Plc",knownAs:"Unity Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535526/Banks/UBA.png",code:"215",isAvailable:!0},{id:"22",name:"Wema Bank",knownAs:"Wema Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535532/Banks/WemaBank.png",code:"035",isAvailable:!0},{id:"23",name:"Zenith Bank",knownAs:"Zenith Bank",logo:"https://res.cloudinary.com/dev-lynx/image/upload/v1562535533/Banks/ZenithBank.png",code:"057",isAvailable:!0}]},163:function(e,t,a){"use strict";var n=a(132),r=a(312),o=a(127),i=a(0),l=a.n(i);t.a=function(e){var t=e.text,a=e.image,c=Object(i.useContext)(n.a);return l.a.createElement(r.a,{width:"100%",height:"small"!==c?"400px":"250px",margin:{top:"small"!==c?"-4.5rem":"-1rem"},background:{color:"brand",image:"url(".concat(a,")"),opacity:"strong",position:"center",size:"cover"}},l.a.createElement(r.a,{height:"100%",style:{color:"white"},background:"rgba(51, 51, 51, 0.5)",pad:"large",responsive:!0,direction:"column",justify:"end"},l.a.createElement(o.a,{style:{fontWeight:100}},t)))}},170:function(e,t,a){"use strict";a.d(t,"a",function(){return b});var n=a(137),r=a(138),o=a(142),i=a(139),l=a(156),c=a(141),s=a(312),d=a(206),u=a(307),m=a(201),p=a(282),f=a(0),g=a.n(f),h=a(140),v=a.n(h),y=a(147),b=function(e){function t(e){var a;return Object(n.a)(this,t),(a=Object(o.a)(this,Object(i.a)(t).call(this,e))).state={date:a.props.date,szDate:v()(a.props.date).local().format("DD/MM/YYYY")},a.renderCalendar=function(){return g.a.createElement(s.a,{pad:"small"},g.a.createElement(s.a,{direction:"row",gap:"2px",align:"center",pad:{vertical:"small"}},g.a.createElement(s.a,{width:"32px"},g.a.createElement("i",{className:"zwicon-calendar-month",style:{color:"#9060EB"}})),g.a.createElement(d.a,{value:a.state.szDate,mask:y.b.date,onChange:function(e){var t=e.target.value;a.setState({szDate:t})},onBlur:function(e){try{var t=v()(e.target.value,"DD/MM/YYYY"),n=t.toDate();a.setState({szDate:t.format("l"),date:n}),a.props.onChange&&a.props.onChange({target:Object(l.a)(a),value:n})}catch(r){}}})),g.a.createElement(s.a,{margin:{top:"small"}},g.a.createElement(u.a,{color:"secondary",size:a.props.calendarSize?a.props.calendarSize:"medium",date:a.props.date.toISOString(),onSelect:function(e){var t=v()(new Date(e)),n=t.toDate();a.setState({szDate:t.format("l"),date:n}),a.props.onChange&&a.props.onChange({target:Object(l.a)(a),value:n})}})))},a}return Object(c.a)(t,e),Object(r.a)(t,[{key:"render",value:function(){var e=this;return g.a.createElement(s.a,{width:"180px"},g.a.createElement(m.a,Object.assign({open:this.props.open,onClose:function(){return e.setState({open:!1})},onOpen:function(){return e.setState({open:!0})},dropContent:this.renderCalendar()},this.props),g.a.createElement(s.a,{direction:"row",gap:"2px",align:"center",justify:"start",pad:{vertical:"small"}},g.a.createElement(s.a,{width:"32px"},g.a.createElement("i",{className:"zwicon-calendar-month",style:{color:"#9060EB"}})),g.a.createElement(s.a,null,g.a.createElement(p.a,{width:"auto",value:new Date(this.props.date).toDateString()})))))}}]),t}(f.Component)},189:function(e,t,a){"use strict";a.d(t,"a",function(){return m});var n=a(137),r=a(138),o=a(142),i=a(139),l=a(141),c=a(292),s=a(0),d=a.n(s),u=13,m=function(e){function t(e){var a;return Object(n.a)(this,t),(a=Object(o.a)(this,Object(i.a)(t).call(this,e))).timer=void 0,a.state={event:{},value:""},a.properties=void 0,a.handleChange=function(e){e.persist(),a.properties.onUndelayedChange&&a.properties.onUndelayedChange(e),clearTimeout(a.timer),a.setState({event:e,value:e.target.value}),a.timer=setTimeout(a.triggerChange,a.properties.waitInterval)},a.handleKeyDown=function(e){e.keyCode===u&&(clearTimeout(a.timer),a.triggerChange())},a.triggerChange=function(){var e=a.state.event;a.properties.onChange(e)},a.properties=a.props,a}return Object(l.a)(t,e),Object(r.a)(t,[{key:"componentWillMount",value:function(){this.timer=null}},{key:"render",value:function(){var e=this;return d.a.createElement(c.a,{label:this.properties.label,placeholder:this.properties.placeholder,value:this.state.value,onChange:this.handleChange,onKeyDown:this.handleKeyDown,onBlur:function(t){e.props.throwOnLostFocus&&(t.persist(),e.props.onChange(t))},component:this.props.component},this.props.children)}}]),t}(s.Component)},190:function(e,t,a){e.exports=a.p+"static/media/dark-dashboard.01dfa71b.jpg"},194:function(e,t,a){e.exports=a.p+"static/media/eastwood-no-messages.5d78e7a6.png"},195:function(e,t,a){"use strict";a.d(t,"a",function(){return y});var n=a(137),r=a(138),o=a(142),i=a(139),l=a(141),c=a(0),s=a.n(c),d=a(312),u=a(205),m=a(314),p=a(140),f=a.n(p),g=a(170),h={name:"Custom",range:function(){return{}}},v=[{name:"Today",range:function(){return{start:f()().startOf("day").toDate(),end:f()().endOf("day").toDate()}}},{name:"Yesterday",range:function(){var e=f()().subtract(1,"days");return{start:e.startOf("day").toDate(),end:e.endOf("day").toDate()}}},{name:"This week",range:function(){var e=f()();return{start:e.startOf("week").toDate(),end:e.endOf("week").toDate()}}},{name:"Last week",range:function(){var e=f()().subtract(1,"weeks");return{start:e.startOf("week").toDate(),end:e.endOf("week").toDate()}}},{name:"This month",range:function(){var e=f()();return{start:e.startOf("month").toDate(),end:e.endOf("month").toDate()}}},{name:"Last Month",range:function(){var e=f()().subtract(1,"months");return{start:e.startOf("month").toDate(),end:e.endOf("month").toDate()}}},{name:"Last 3 months",range:function(){return{start:f()().subtract(3,"months").startOf("month").toDate(),end:f()().endOf("month").toDate()}}},{name:"This year",range:function(){return{start:f()().startOf("year").toDate(),end:f()().endOf("year").toDate()}}},{name:"Last year",range:function(){var e=f()().subtract(1,"years");return{start:e.startOf("year").toDate(),end:e.endOf("year").toDate()}}},{name:"All time",range:function(){return{start:f()("2000","YYYY").startOf("year").toDate(),end:f()().endOf("year").toDate()}}}],y=function(e){function t(){var e,a;Object(n.a)(this,t);for(var r=arguments.length,l=new Array(r),c=0;c<r;c++)l[c]=arguments[c];return(a=Object(o.a)(this,(e=Object(i.a)(t)).call.apply(e,[this].concat(l)))).state={range:v[4],dateRange:{start:v[4].range().start,end:v[4].range().end}},a.renderCount=0,a}return Object(l.a)(t,e),Object(r.a)(t,[{key:"onChange",value:function(){this.props.onChange&&this.props.onChange(this.state.dateRange,this)}},{key:"render",value:function(){var e=this;return this.renderCount<=0&&this.onChange(),this.renderCount++,s.a.createElement(d.a,null,s.a.createElement(d.a,{width:"auto",alignSelf:"start"},s.a.createElement(u.a,{value:this.state.range.name,options:v.map(function(e){return e.name}),onChange:function(t){var a=v[t.selected];"Custom"!=a.name&&e.setState({range:a,dateRange:a.range()},e.onChange)}})),s.a.createElement(d.a,{direction:"row",align:"center",alignSelf:"start",gap:"small"},s.a.createElement(m.a,null,"From"),s.a.createElement(g.a,{date:this.state.dateRange.start,onChange:function(t){e.setState({dateRange:{start:f()(t.value).startOf("day").toDate(),end:e.state.dateRange.end},range:h},e.onChange)}}),s.a.createElement(m.a,null,"To"),s.a.createElement(g.a,{date:this.state.dateRange.end,onChange:function(t){e.setState({dateRange:{start:e.state.dateRange.start,end:f()(t.value).endOf("day").toDate()},range:h},e.onChange)}})))}}]),t}(c.Component)},196:function(e,t,a){"use strict";var n=a(0),r=a.n(n);a(197);t.a=function(e){return e.active?r.a.createElement("div",{className:"spinner-box"},r.a.createElement("div",{className:"circle-border"},r.a.createElement("div",{className:"circle-core"}))):null}},197:function(e,t,a){},221:function(e,t,a){"use strict";a.d(t,"a",function(){return i});var n=a(0),r=a.n(n),o=a(145);var i=function(e){var t=e.caption,a=e.children,n=function(e,t){if(null==e)return{};var a,n,r={},o=Object.keys(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||(r[a]=e[a]);return r}(e,["caption","children"]);return r.a.createElement(o.a,n,t?r.a.createElement(o.d,null,t):null,a)}},222:function(e,t,a){"use strict";a.d(t,"a",function(){return l});var n=a(0),r=a.n(n),o=a(153),i=a(145);var l=function(e){return r.a.createElement(o.a.Provider,{value:"header"},r.a.createElement(i.e,e))}},223:function(e,t,a){"use strict";a.d(t,"a",function(){return i});var n=a(0),r=a.n(n),o=a(145);var i=function(e){return r.a.createElement(o.f,e)}},224:function(e,t,a){"use strict";a.d(t,"a",function(){return p});var n=a(0),r=a.n(n),o=a(135),i=a(1),l=a(22),c=a(312),s=a(153),d=a(145);function u(){return(u=Object.assign||function(e){for(var t=1;t<arguments.length;t++){var a=arguments[t];for(var n in a)Object.prototype.hasOwnProperty.call(a,n)&&(e[n]=a[n])}return e}).apply(this,arguments)}var m=function(e){var t=e.children,a=e.colSpan,n=e.plain,o=e.scope,i=e.size,l=e.theme,m=e.verticalAlign,p=function(e,t){if(null==e)return{};var a,n,r={},o=Object.keys(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||(r[a]=e[a]);return r}(e,["children","colSpan","plain","scope","size","theme","verticalAlign"]);return r.a.createElement(s.a.Consumer,null,function(e){var s;s="header"===e?l.table&&l.table.header:"footer"===e?l.table&&l.table.footer:l.table&&l.table.body;var f=u({},p);return Object.keys(f).forEach(function(e){s[e]&&void 0===f[e]&&delete f[e]}),r.a.createElement(d.c,u({as:o?"th":void 0,scope:o,size:i,colSpan:a,tableContext:e,tableContextTheme:s,verticalAlign:m||(s?s.verticalAlign:void 0)},n?p:{}),n?t:r.a.createElement(c.a,u({},s,f),t))})};m.defaultProps={},Object.setPrototypeOf(m.defaultProps,l.a);var p=Object(o.a)(i.withTheme)(m)},225:function(e,t,a){"use strict";a.d(t,"a",function(){return l});var n=a(0),r=a.n(n),o=a(153),i=a(145);var l=function(e){return r.a.createElement(o.a.Provider,{value:"body"},r.a.createElement(i.b,e))}},300:function(e,t,a){"use strict";a.r(t);var n=a(30),r=a(50),o=a(24),i=a(1),l=a(312),c=a(132),s=a(291),d=a(205),u=a(315),m=a(127),p=a(314),f=a(221),g=a(222),h=a(223),v=a(224),y=a(225),b=a(0),k=a.n(b),w=a(163),E=a(190),O=a.n(E),B=a(195),x=a(189),A=a(196),C=a(14),D=a.n(C),S=a(194),j=a.n(S),T=a(157),P=a(146),z=a(149),N=a(140),$=a.n(N);function Y(){var e=Object(o.a)(["\n\twidth: 100vw;\n\talign-items: center;\n    padding-bottom: 2rem;\n    min-height: 720px;\n    \n\t@media (min-width: 768px) {\n\t\t// align-items: start;\n\t}\n"]);return Y=function(){return e},e}var L=Object(i.default)(l.a)(Y());t.default=function(){var e,t,a,o=Object(b.useContext)(c.a),i=Object(b.useState)(!1),E=Object(r.a)(i,2),C=E[0],S=E[1],N=Object(b.useState)({}),Y=Object(r.a)(N,2),F=Y[0],I=Y[1],U=Object(b.useState)({}),_=Object(r.a)(U,2),M=_[0],R=_[1],H=Object(b.useState)({start:new Date,end:new Date}),K=Object(r.a)(H,2),W=K[0],J=K[1],Z=Object(b.useState)(""),q=Object(r.a)(Z,2),G=q[0],Q=q[1],V=Object(b.useState)({name:"Deposits",path:"/api/backOffice/Transactions",filter:"type==deposit"}),X=Object(r.a)(V,2),ee=X[0],te=X[1],ae=Object(b.useState)([]),ne=Object(r.a)(ae,2),re=ne[0],oe=ne[1],ie=Object(b.useState)(!1),le=Object(r.a)(ie,2),ce=le[0],se=le[1],de=[{name:"Deposits",path:"/api/backOffice/Transactions",filter:"type==Deposit"},{name:"Payment Claims",path:"/api/backOffice/Transactions/requests",filter:""},{name:"Pending Claims",path:"/api/backOffice/Transactions/requests",filter:"status==Pending"}];Object(b.useEffect)(function(){ue()},[G,W,ee]);var ue=function n(){if(t)return a=!0,void e();t=!0,S(!0),D.a.get(ee.path,{params:{start:W.start,end:W.end,searchQuery:G,filters:ee.filter},cancelToken:new D.a.CancelToken(function(t){return e=t})}).then(function(e){if(200==e.status){var t=e.data;t="/api/backOffice/Transactions"==ee.path?e.data.sort(function(e,t){var a=new Date(e.addedAt),n=new Date(t.addedAt);return a>n?-1:a<n?1:0}):e.data.sort(function(e,t){var a=new Date(e.created),n=new Date(t.created);return a>n?-1:a<n?1:0}),console.log(t),oe(t)}}).catch(function(e){console.log(e)}).finally(function(){t=!1,a?(a=!1,n()):setTimeout(function(){return S(!1)},1e3)})};return k.a.createElement(L,{pad:{bottom:"2rem"},align:"center"},k.a.createElement(P.a,{message:F.message,show:F.show,variant:F.variant,onClose:function(){return I(function(e){return Object(n.a)({},e,{show:!1})})}}),k.a.createElement(w.a,{image:O.a,text:"Transactions"}),k.a.createElement(z.a,{title:M.title,open:M.open,message:M.message,action:M.action,size:o,onClose:function(){R({open:!1,message:"",action:function(){}})}}),k.a.createElement(l.a,{justify:"center",align:"center"},k.a.createElement(l.a,{pad:"large",width:"small"!==o?"90vw":"80vw",background:"white",overflow:{horizontal:"auto"},align:"center",justify:"center",round:"small",elevation:"medium",margin:{top:"large"}},k.a.createElement(l.a,{width:"100%"},k.a.createElement(l.a,{width:"100%"},k.a.createElement(s.a,null,k.a.createElement(l.a,{width:"100%",direction:"row",margin:{vertical:"small"},justify:"between"},k.a.createElement(B.a,{onChange:function(e,t){J({start:e.start,end:e.end})}}),k.a.createElement(l.a,{direction:"row",gap:"medium",pad:{top:"small"},alignSelf:"end"},k.a.createElement(l.a,{direction:"row",gap:"2px",pad:{top:"small"},margin:{bottom:"small"},alignSelf:"end"},k.a.createElement(l.a,{width:"32px",color:"#9060EB",alignSelf:"center"},k.a.createElement("i",{className:"zwicon-filter",style:{color:"#9060EB"}})),k.a.createElement(d.a,{value:ee.name,options:de.map(function(e){return e.name}),onChange:function(e){var t=de[e.selected];te(t),se("/api/backOffice/Transactions/requests"==t.path)}})),k.a.createElement(l.a,{direction:"row",gap:"2px",pad:{top:"small"},alignSelf:"end"},k.a.createElement(l.a,{width:"32px",color:"#9060EB",alignSelf:"center"},k.a.createElement("i",{className:"zwicon-search",style:{color:"#9060EB"}})),k.a.createElement(x.a,{waitInterval:3e3,placeholder:"Search",value:G,onChange:function(e){Q(e.target.value)}}))))))),k.a.createElement(l.a,{width:"100%",style:{position:"relative",minHeight:"500px"},margin:{top:"30px"}},C||re&&!(re.length<=0)?null:k.a.createElement(l.a,{height:"100%",width:"100%",align:"center",justify:"center",style:{position:"absolute",top:0,left:0}},k.a.createElement(l.a,{height:"300px",width:"auto"},k.a.createElement(u.a,{src:j.a,fit:"contain"})),k.a.createElement(l.a,{width:"500px",align:"center",justify:"center"},k.a.createElement(m.a,{margin:{top:"0px"},level:"4"},"Huh? What happened to the Postman?"),k.a.createElement(p.a,{weight:400,size:"16px",textAlign:"center"},"There doesn't seem to be any data to display, try searching or filtering to find what you want."))),k.a.createElement(A.a,{active:C}),re&&!C?k.a.createElement(k.a.Fragment,null,ce?k.a.createElement(f.a,{cellPadding:"medium",className:"hoverTable"},k.a.createElement(g.a,{style:{borderBottom:"solid 1px #ccc",fontSize:"14px !important"}},k.a.createElement(h.a,null,k.a.createElement(v.a,{scope:"col"},"Created"),k.a.createElement(v.a,{scope:"col"},"Transaction Date"),k.a.createElement(v.a,{scope:"col"},"User"),k.a.createElement(v.a,{scope:"col"},"Amount"),k.a.createElement(v.a,{scope:"col"},"Payment Channel"),k.a.createElement(v.a,{scope:"col"},"Paid To"),k.a.createElement(v.a,{scope:"col"},"Paid From"),k.a.createElement(v.a,{scope:"col"},"Description"),k.a.createElement(v.a,{scope:"col"},"Status"))),k.a.createElement(y.a,null,re.map(function(e,t,a){return k.a.createElement(h.a,{key:t,style:{padding:".1rem"}},k.a.createElement(v.a,null,new Date(e.created).toLocaleDateString()),k.a.createElement(v.a,null,new Date(e.transactionDate).toLocaleDateString()),k.a.createElement(v.a,null,e.username),k.a.createElement(v.a,null,"\u20a6 "+e.amount.toLocaleString()),k.a.createElement(v.a,null,e.paymentChannel.name),k.a.createElement(v.a,null,e.platformBankAccount?e.platformBankAccount.accountName+"              "+e.platformBankAccount.accountNumber+"              "+T[e.platformBankAccount.bankId-1].knownAs:""),k.a.createElement(v.a,null,e.userBankAccount?e.userBankAccount.accountName+"              "+e.userBankAccount.accountNumber+"              "+T[e.userBankAccount.bankId-1].knownAs:""),k.a.createElement(v.a,null,e.description),k.a.createElement(v.a,null,k.a.createElement(l.a,{pad:"0px",onClick:function(){}},k.a.createElement(d.a,{value:e.status,valueLabel:k.a.createElement(l.a,{round:"small",overflow:"hidden",align:"center"},e.status),icon:!0,disabled:"Pending"!=e.status,options:["Approved","Declined","Pending"],onChange:function(t){var a=t.option;"Pending"!=a&&R({open:!0,title:"".concat("Approved"==a?"Approve":"Decline"," this transaction?"),message:"Once a transaction is approved, the user's wallet will be credited with the claimed amount. This action cannot be reversed.",action:function(){return t=e.id,n=a,S(!0),void D.a.post("/api/backOffice/Transactions/requests/update",{id:t,status:n}).then(function(e){200==e.status&&I({message:"Operation successful",variant:"success",show:!0})}).catch(function(e){I({message:"An unexpected error occured, Please try again",variant:"error",show:!0})}).finally(function(){ue()});var t,n}})},plain:!0}))))}))):k.a.createElement(f.a,{cellPadding:"medium",className:"hoverTable"},k.a.createElement(g.a,{style:{borderBottom:"solid 1px #ccc",fontSize:"14px !important"}},k.a.createElement(h.a,null,k.a.createElement(v.a,{scope:"col"},"Created"),k.a.createElement(v.a,{scope:"col"},"Amount"),k.a.createElement(v.a,{scope:"col"},"Payment Channel"),k.a.createElement(v.a,{scope:"col"},"Type"),k.a.createElement(v.a,{scope:"col"},"User"))),k.a.createElement(y.a,null,re.map(function(e,t,a){return k.a.createElement(h.a,{key:t,style:{padding:".1rem"}},k.a.createElement(v.a,null,$()(e.addedAt).format("lll")),k.a.createElement(v.a,null,"\u20a6 "+e.amount.toLocaleString()),k.a.createElement(v.a,null,e.paymentChannel.name),k.a.createElement(v.a,null,e.type),k.a.createElement(v.a,null,e.auxilaryUser.username))})))):null))))}}}]);
//# sourceMappingURL=15.cb3b3e23.chunk.js.map