(window.webpackJsonp=window.webpackJsonp||[]).push([[26],{226:function(e,t,a){"use strict";var n=a(24),r=a(312),o=a(132),i=a(127),l=a(0),c=a.n(l),u=a(1),d=a(274),m=a.n(d),s=a(51);function b(){var e=Object(n.a)(["\n\tcolor: white;\n\t@media (max-width: 768px) {\n\t\theight: 150px;\n\t}\n"]);return b=function(){return e},e}Object(u.default)(r.a)(b());t.a=function(){var e=Object(l.useContext)(o.a),t=Object(l.useContext)(s.c).userState;return c.a.createElement(r.a,{width:"100%",height:"small"!==e?"400px":"250px",margin:{top:"small"!==e?"-4.5rem":"-1rem"},background:{color:"brand",image:"url(".concat(m.a,")"),opacity:"strong",position:"center",size:"cover"}},c.a.createElement(r.a,{height:"100%",style:{color:"white"},background:"rgba(51, 51, 51, 0.5)",pad:"large",responsive:!0,direction:"column",justify:"end"},c.a.createElement(i.a,{style:{fontWeight:100,marginBottom:"-1.5rem"}},"ACCOUNT"),c.a.createElement(i.a,{level:"1"},"\u20a6 "+t.balance.toLocaleString())))}},232:function(e,t,a){"use strict";a.d(t,"a",function(){return u});var n=a(24),r=a(309),o=a(0),i=a.n(o),l=(a(31),a(1));function c(){var e=Object(n.a)(["\n\tborder-radius: 5px;\n\tbackground-color: #9060EB;\n\tcolor: white;\n\ttext-align: center;\n\toutline: none;\n\tmargin-right: 1rem;\n\tborder: none;\n\tbox-shadow: 0px 2px 4px #9060EB;\n\t&: hover {\n\t\tbackground-color: white;\n\t\tcolor: #24501F;\n\t}\n"]);return c=function(){return e},e}Object(l.default)(r.a)(c());var u=function(e){var t=e.history;return i.a.createElement(r.a,{onClick:function(e){return t.push("/dashboard/wallet/deposit")},style:{width:"150px",color:"#fff"},color:"secondary",label:"Deposit",primary:!0})}},274:function(e,t,a){e.exports=a.p+"static/media/wallet.4b027e3a.jpg"},311:function(e,t,a){"use strict";a.r(t);var n=a(24),r=a(13),o=a(312),i=a(132),l=a(314),c=a(0),u=a.n(c),d=a(12),m=a(1),s=a(232),b=a(15),h=a(221),p=a(225),w=a(223),f=a(224),g=a(140),x=a.n(g);function E(){var e=Object(n.a)(["\n\tmax-width: 720px;\n\tmargin: 3rem auto 0;\n"]);return E=function(){return e},e}var v=Object(m.default)(o.a)(E()),y=function(e){var t=e.transactions,a=Object(c.useContext)(i.a);return u.a.createElement(v,{background:"white",pad:"medium",elevation:"medium"},u.a.createElement(h.a,{style:{width:window.innerWidth>720?680:window.innerWidth-100}},u.a.createElement(p.a,null,t.slice(0,10).map(function(e,t,n){var r=t===n.length-1;return u.a.createElement(w.a,{key:e.id},u.a.createElement(f.a,{align:"left",style:{borderBottom:r?"none":"solid 1px rgba(0, 0, 0, 0.3)"}},"small"==a?x()(e.addedAt).format("l"):x()(e.addedAt).format("llll")),u.a.createElement(f.a,{style:{borderBottom:r?"none":"solid 1px rgba(0, 0, 0, 0.3)"}},e.type),u.a.createElement(f.a,{style:{color:"Withdrawal"===e.type||"Debit"===e.type?"#D9251B":"#009746",borderBottom:r?"none":"solid 1px rgba(0, 0, 0, 0.3)"}},"Withdrawal"===e.type||"Debit"===e.type?"-"+e.amount.toLocaleString():e.amount.toLocaleString()))}))))},j=a(226),O=a(51);function k(){var e=Object(n.a)(["\n\twidth: 100vw;\n\tpadding-bottom: 2rem;\n"]);return k=function(){return e},e}var C=Object(r.a)(function(){return Promise.all([a.e(4),a.e(22)]).then(a.bind(null,303))},{fallback:u.a.createElement(b.a,{show:!0})}),B=Object(r.a)(function(){return a.e(31).then(a.bind(null,310))},{fallback:u.a.createElement(b.a,{show:!0})}),S=Object(m.default)(o.a)(k());t.default=function(e){var t=e.history,a=Object(c.useContext)(i.a),n=Object(c.useContext)(O.c).userState;return u.a.createElement(u.a.Fragment,null,u.a.createElement(d.a,{exact:!0,path:"/dashboard/wallet",render:function(){return u.a.createElement(S,{direction:"column",align:"center"},u.a.createElement(j.a,null),u.a.createElement(o.a,{direction:"row",margin:"medium",width:"100%",justify:"small"!==a?"end":"center",pad:{horizontal:"xlarge"},gap:"medium"},u.a.createElement(s.a,{history:t})),u.a.createElement(o.a,{width:"small"!==a?"720px":"80vw",direction:"row",margin:{top:"xlarge",bottom:"medium"}},u.a.createElement(l.a,{size:"xxlarge",weight:100},"Transactions")),u.a.createElement(o.a,{width:"100vw"},u.a.createElement(y,{transactions:n.transactions})))}}),u.a.createElement(d.a,{path:"/dashboard/wallet/deposit",component:C}),u.a.createElement(d.a,{path:"/dashboard/wallet/withdraw",component:B}))}}}]);
//# sourceMappingURL=26.c168b1d3.chunk.js.map