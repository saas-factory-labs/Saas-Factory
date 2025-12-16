import { computed as c, inject as ze, defineComponent as ie, createElementBlock as i, openBlock as a, mergeProps as ke, withModifiers as He, renderSlot as H, ref as T, unref as W, isRef as ss, nextTick as At, provide as rl, watchEffect as kl, normalizeClass as m, createElementVNode as n, createCommentVNode as x, toDisplayString as M, h as Vt, resolveComponent as z, createBlock as X, withCtx as xe, useAttrs as fa, createVNode as he, createTextVNode as we, normalizeStyle as un, createStaticVNode as Sl, Fragment as $e, renderList as Se, withDirectives as It, vModelCheckbox as dn, withKeys as as, vModelSelect as va, useSlots as Ml, getCurrentInstance as Oe, onMounted as Ye, createSlots as cn, normalizeProps as Cl, guardReactiveProps as os, vModelDynamic as ma, onUnmounted as jt, watch as xt, vModelText as ga, resolveDynamicComponent as is, resolveDirective as pa } from "vue";
import { lastRightPart as Dt, leftPart as Tl, map as Ne, toDate as gt, mapGet as be, toCamelCase as ya, toDateTime as ha, chop as ba, isDate as Fl, dateFmt as wa, fromXsdDuration as rs, timeFmt12 as xa, omit as ft, appendQueryString as Qt, indexOfAny as ka, apiValue as Ca, enc as Gl, createBus as $a, toKebabCase as Wn, ApiResult as We, nameOf as La, lastLeftPart as us, setQueryString as Va, toTime as Sa, ResponseStatus as Kl, ResponseError as Gn, sanitize as Ma, errorResponseExcept as Ta, humanize as Pe, delaySet as ds, rightPart as hl, queryString as Jl, combinePaths as Fa, toPascalCase as ot, errorResponse as pt, trimEnd as Aa, $1 as $l, HttpMethods as fn, omitEmpty as Ia, uniqueKeys as Xl, humanify as cs, each as Da } from "@servicestack/client";
const fs = "png,jpg,jpeg,jfif,gif,svg,webp".split(","), vs = {
  img: "png,jpg,jpeg,gif,svg,webp,png,jpg,jpeg,gif,bmp,tif,tiff,webp,ai,psd,ps".split(","),
  vid: "avi,m4v,mov,mp4,mpg,mpeg,wmv,webm".split(","),
  aud: "mp3,mpa,ogg,wav,wma,mid,webm".split(","),
  ppt: "key,odp,pps,ppt,pptx".split(","),
  xls: "xls,xlsm,xlsx,ods,csv,tsv".split(","),
  doc: "doc,docx,pdf,rtf,tex,txt,md,rst,xls,xlsm,xlsx,ods,key,odp,pps,ppt,pptx".split(","),
  zip: "zip,tar,gz,7z,rar,gzip,deflate,br,iso,dmg,z,lz,lz4,lzh,s7z,apl,arg,jar,war".split(","),
  exe: "exe,bat,sh,cmd,com,app,msi,run,vb,vbs,js,ws,wsh".split(","),
  att: "bin,oct,dat".split(",")
  //attachment
}, Jn = Object.keys(vs), mt = (e, t) => `<svg xmlns='http://www.w3.org/2000/svg' aria-hidden='true' role='img' preserveAspectRatio='xMidYMid meet' viewBox='${e}'>${t}</svg>`, bl = {
  img: mt("4 4 16 16", "<path fill='currentColor' d='M20 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2zm-2 0H6v6.38l2.19-2.19l5.23 5.23l1-1a1.59 1.59 0 0 1 2.11.11L18 16V6zm-5 3.5a1.5 1.5 0 1 1 3 0a1.5 1.5 0 0 1-3 0z'/>"),
  vid: mt("0 0 24 24", "<path fill='currentColor' d='m14 2l6 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8m4 18V9h-5V4H6v16h12m-2-2l-2.5-1.7V18H8v-5h5.5v1.7L16 13v5Z'/>"),
  aud: mt("0 0 24 24", "<path fill='currentColor' d='M14 2H6c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8l-6-6zM6 20V4h7v5h5v11H6zm10-9h-4v3.88a2.247 2.247 0 0 0-3.5 1.87c0 1.24 1.01 2.25 2.25 2.25S13 17.99 13 16.75V13h3v-2z'/>"),
  ppt: mt("0 0 48 48", "<g fill='none' stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='4'><path d='M4 8h40'/><path d='M8 8h32v26H8V8Z' clip-rule='evenodd'/><path d='m22 16l5 5l-5 5m-6 16l8-8l8 8'/></g>"),
  xls: mt("0 0 256 256", "<path fill='currentColor' d='M200 26H72a14 14 0 0 0-14 14v26H40a14 14 0 0 0-14 14v96a14 14 0 0 0 14 14h18v26a14 14 0 0 0 14 14h128a14 14 0 0 0 14-14V40a14 14 0 0 0-14-14Zm-42 76h44v52h-44Zm44-62v50h-44V80a14 14 0 0 0-14-14h-2V38h58a2 2 0 0 1 2 2ZM70 40a2 2 0 0 1 2-2h58v28H70ZM38 176V80a2 2 0 0 1 2-2h104a2 2 0 0 1 2 2v96a2 2 0 0 1-2 2H40a2 2 0 0 1-2-2Zm32 40v-26h60v28H72a2 2 0 0 1-2-2Zm130 2h-58v-28h2a14 14 0 0 0 14-14v-10h44v50a2 2 0 0 1-2 2ZM69.2 148.4L84.5 128l-15.3-20.4a6 6 0 1 1 9.6-7.2L92 118l13.2-17.6a6 6 0 0 1 9.6 7.2L99.5 128l15.3 20.4a6 6 0 0 1-9.6 7.2L92 138l-13.2 17.6a6 6 0 1 1-9.6-7.2Z'/>"),
  doc: mt("0 0 32 32", "<path fill='currentColor' d='M26 30H11a2.002 2.002 0 0 1-2-2v-6h2v6h15V6h-9V4h9a2.002 2.002 0 0 1 2 2v22a2.002 2.002 0 0 1-2 2Z'/><path fill='currentColor' d='M17 10h7v2h-7zm-1 5h8v2h-8zm-1 5h9v2h-9zm-6-1a5.005 5.005 0 0 1-5-5V3h2v11a3 3 0 0 0 6 0V5a1 1 0 0 0-2 0v10H8V5a3 3 0 0 1 6 0v9a5.005 5.005 0 0 1-5 5z'/>"),
  zip: mt("0 0 16 16", "<g fill='currentColor'><path d='M6.5 7.5a1 1 0 0 1 1-1h1a1 1 0 0 1 1 1v.938l.4 1.599a1 1 0 0 1-.416 1.074l-.93.62a1 1 0 0 1-1.109 0l-.93-.62a1 1 0 0 1-.415-1.074l.4-1.599V7.5zm2 0h-1v.938a1 1 0 0 1-.03.243l-.4 1.598l.93.62l.93-.62l-.4-1.598a1 1 0 0 1-.03-.243V7.5z'/><path d='M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2zm5.5-1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H9v1H8v1h1v1H8v1h1v1H7.5V5h-1V4h1V3h-1V2h1V1z'/></g>"),
  exe: mt("0 0 16 16", "<path fill='currentColor' fill-rule='evenodd' d='M14 4.5V14a2 2 0 0 1-2 2h-1v-1h1a1 1 0 0 0 1-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5L14 4.5ZM2.575 15.202H.785v-1.073H2.47v-.606H.785v-1.025h1.79v-.648H0v3.999h2.575v-.647ZM6.31 11.85h-.893l-.823 1.439h-.036l-.832-1.439h-.931l1.227 1.983l-1.239 2.016h.861l.853-1.415h.035l.85 1.415h.908l-1.254-1.992L6.31 11.85Zm1.025 3.352h1.79v.647H6.548V11.85h2.576v.648h-1.79v1.025h1.684v.606H7.334v1.073Z'/>"),
  att: mt("0 0 24 24", "<path fill='currentColor' d='M14 0a5 5 0 0 1 5 5v12a7 7 0 1 1-14 0V9h2v8a5 5 0 0 0 10 0V5a3 3 0 1 0-6 0v12a1 1 0 1 0 2 0V6h2v11a3 3 0 1 1-6 0V5a5 5 0 0 1 5-5Z'/>")
}, Oa = /[\r\n%#()<>?[\\\]^`{|}]/g, Xn = 1024, Pa = ["Bytes", "KB", "MB", "GB", "TB"], ja = (() => {
  const e = "application/", t = e + "vnd.openxmlformats-officedocument.", l = "image/", s = "text/", o = "audio/", r = "video/", u = {
    jpg: l + "jpeg",
    tif: l + "tiff",
    svg: l + "svg+xml",
    ico: l + "x-icon",
    ts: s + "typescript",
    py: s + "x-python",
    sh: s + "x-sh",
    mp3: o + "mpeg3",
    mpg: r + "mpeg",
    ogv: r + "ogg",
    xlsx: t + "spreadsheetml.sheet",
    xltx: t + "spreadsheetml.template",
    docx: t + "wordprocessingml.document",
    dotx: t + "wordprocessingml.template",
    pptx: t + "presentationml.presentation",
    potx: t + "presentationml.template",
    ppsx: t + "presentationml.slideshow",
    mdb: e + "vnd.ms-access"
  };
  function d(v, y) {
    v.split(",").forEach((L) => u[L] = y);
  }
  function f(v, y) {
    v.split(",").forEach((L) => u[L] = y(L));
  }
  return f("jpeg,gif,png,tiff,bmp,webp", (v) => l + v), f("jsx,csv,css", (v) => s + v), f("aac,ac3,aiff,m4a,m4b,m4p,mid,midi,wav", (v) => o + v), f("3gpp,avi,dv,divx,ogg,mp4,webm", (v) => r + v), f("rtf,pdf", (v) => e + v), d("htm,html,shtm", s + "html"), d("js,mjs,cjs", s + "javascript"), d("yml,yaml", e + "yaml"), d("bat,cmd", e + "bat"), d("xml,csproj,fsproj,vbproj", s + "xml"), d("txt,ps1", s + "plain"), d("qt,mov", r + "quicktime"), d("doc,dot", e + "msword"), d("xls,xlt,xla", e + "excel"), d("ppt,oit,pps,ppa", e + "vnd.ms-powerpoint"), d("cer,crt,der", e + "x-x509-ca-cert"), d("gz,tgz,zip,rar,lzh,z", e + "x-compressed"), d("aaf,aca,asd,bin,cab,chm,class,cur,db,dat,deploy,dll,dsp,exe,fla,ics,inf,mix,msi,mso,obj,ocx,prm,prx,psd,psp,qxd,sea,snp,so,sqlite,toc,ttf,u32,xmp,xsn,xtp", e + "octet-stream"), u;
})();
let Yl = [];
function ms(e) {
  return e = e.replace(/"/g, "'"), e = e.replace(/>\s+</g, "><"), e = e.replace(/\s{2,}/g, " "), e.replace(Oa, encodeURIComponent);
}
function vn(e) {
  return "data:image/svg+xml;utf8," + ms(e);
}
function gs(e) {
  let t = URL.createObjectURL(e);
  return Yl.push(t), t;
}
function ps() {
  Yl.forEach((e) => {
    try {
      URL.revokeObjectURL(e);
    } catch (t) {
      console.error("URL.revokeObjectURL", t);
    }
  }), Yl = [];
}
function mn(e) {
  if (!e) return null;
  let t = Tl(e, "?");
  return Dt(t, "/");
}
function ul(e) {
  let t = mn(e);
  return t == null || t.indexOf(".") === -1 ? null : Dt(t, ".").toLowerCase();
}
function gn(e) {
  let t = ul(e.name);
  return t && fs.indexOf(t) >= 0 ? gs(e) : St(e.name);
}
function pn(e) {
  if (!e) return !1;
  if (e.startsWith("blob:") || e.startsWith("data:"))
    return !0;
  let t = ul(e);
  return t && fs.indexOf(t) >= 0 || !1;
}
function St(e) {
  if (!e) return null;
  let t = ul(e);
  return t == null || pn(e) ? e : tl(t) || vn(bl.doc);
}
function tl(e) {
  let t = ys(e);
  return t && vn(t) || null;
}
function ys(e) {
  if (bl[e])
    return bl[e];
  for (let t = 0; t < Jn.length; t++) {
    let l = Jn[t];
    if (vs[l].indexOf(e) >= 0)
      return bl[l];
  }
  return null;
}
function yn(e, t = 2) {
  if (e === 0) return "0 Bytes";
  const l = t < 0 ? 0 : t, s = Math.floor(Math.log(e) / Math.log(Xn));
  return parseFloat((e / Math.pow(Xn, s)).toFixed(l)) + " " + Pa[s];
}
function Ba(e) {
  return e.files && Array.from(e.files).map((t) => ({ fileName: t.name, contentLength: t.size, filePath: gn(t) }));
}
function Al(e, t) {
  e.onerror = null, e.src = hn(e.src, t) || "";
}
function hn(e, t) {
  return tl(Dt(e, ".").toLowerCase()) || (t ? tl(t) || t : null) || tl("doc");
}
function en(e) {
  if (!e)
    throw new Error("fileNameOrExt required");
  const t = Dt(e, ".").toLowerCase();
  return ja[t] || "application/" + t;
}
function Ha() {
  return {
    extSvg: ys,
    extSrc: tl,
    getExt: ul,
    encodeSvg: ms,
    canPreview: pn,
    getFileName: mn,
    getMimeType: en,
    formatBytes: yn,
    filePathUri: St,
    svgToDataUri: vn,
    fileImageUri: gn,
    objectUrl: gs,
    flush: ps,
    inputFiles: Ba,
    iconOnError: Al,
    iconFallbackSrc: hn
  };
}
class Ra {
  view;
  includeTypes;
  constructor(t) {
    Object.assign(this, t);
  }
  getTypeName() {
    return "MetadataApp";
  }
  getMethod() {
    return "GET";
  }
  createResponse() {
    return {};
  }
}
const Kt = "/metadata/app.json", Ea = {
  Boolean: "checkbox",
  DateTime: "date",
  DateOnly: "date",
  DateTimeOffset: "date",
  TimeSpan: "time",
  TimeOnly: "time",
  Byte: "number",
  Short: "number",
  Int64: "number",
  Int32: "number",
  UInt16: "number",
  UInt32: "number",
  UInt64: "number",
  Single: "number",
  Double: "number",
  Decimal: "number",
  String: "text",
  Guid: "text",
  Uri: "text"
}, Na = {
  number: "Int32",
  checkbox: "Boolean",
  date: "DateTime",
  "datetime-local": "DateTime",
  time: "TimeSpan"
}, tn = {
  Byte: "byte",
  Int16: "short",
  Int32: "int",
  Int64: "long",
  UInt16: "ushort",
  Unt32: "uint",
  UInt64: "ulong",
  Single: "float",
  Double: "double",
  Decimal: "decimal"
};
[...Object.keys(tn), ...Object.values(tn)];
const za = {
  String: "string",
  Boolean: "bool",
  ...tn
};
function gl(e) {
  return za[e] || e;
}
function hs(e, t) {
  return e ? (t || (t = []), e === "Nullable`1" ? gl(t[0]) + "?" : e.endsWith("[]") ? `List<${gl(e.substring(0, e.length - 2))}>` : t.length === 0 ? gl(e) : Tl(gl(e), "`") + "<" + t.join(",") + ">") : "";
}
function Ua(e) {
  return e && hs(e.name, e.genericArgs);
}
class Ot {
  Query;
  QueryInto;
  Create;
  Update;
  Patch;
  Delete;
  get AnyQuery() {
    return this.Query || this.QueryInto;
  }
  get AnyUpdate() {
    return this.Patch || this.Update;
  }
  get dataModel() {
    return this.AnyQuery?.dataModel;
  }
  toArray() {
    return [this.Query, this.QueryInto, this.Create, this.Update, this.Patch, this.Delete].filter((l) => !!l).map((l) => l);
  }
  get empty() {
    return !this.Query && !this.QueryInto && !this.Create && !this.Update && !this.Patch && !this.Delete;
  }
  add(t) {
    Ee.isQueryInto(t) && !this.QueryInto ? this.QueryInto = t : Ee.isQuery(t) && !this.Query ? this.Query = t : Ee.isCreate(t) && !this.Create ? this.Create = t : Ee.isUpdate(t) && !this.Update ? this.Update = t : Ee.isPatch(t) && !this.Patch ? this.Patch = t : Ee.isDelete(t) && !this.Delete && (this.Delete = t);
  }
  static from(t) {
    const l = new Ot();
    return t.forEach((s) => {
      l.add(s);
    }), l;
  }
  static forType(t, l) {
    let s = new Ot();
    if (q.config.apisResolver && t) {
      const o = q.config.apisResolver(t, l);
      o && (s.Query = o.Query, s.QueryInto = o.QueryInto, s.Create = o.Create, s.Update = o.Update, s.Patch = o.Patch, s.Delete = o.Delete);
    }
    return t && (l ??= q.metadata.value?.api, l?.operations.forEach((o) => {
      o.dataModel?.name == t && s.add(o);
    })), s;
  }
}
const Ee = {
  Create: "ICreateDb`1",
  Update: "IUpdateDb`1",
  Patch: "IPatchDb`1",
  Delete: "IDeleteDb`1",
  AnyRead: ["QueryDb`1", "QueryDb`2"],
  AnyWrite: ["ICreateDb`1", "IUpdateDb`1", "IPatchDb`1", "IDeleteDb`1"],
  isAnyQuery: (e) => Ne(e.request.inherits, (t) => Ee.AnyRead.indexOf(t.name) >= 0),
  isQuery: (e) => Ne(e.request.inherits, (t) => t.name === "QueryDb`1"),
  isQueryInto: (e) => Ne(e.request.inherits, (t) => t.name === "QueryDb`2"),
  isCrud: (e) => e.request.implements?.some((t) => Ee.AnyWrite.indexOf(t.name) >= 0),
  isCreate: (e) => pl(e, Ee.Create),
  isUpdate: (e) => pl(e, Ee.Update),
  isPatch: (e) => pl(e, Ee.Patch),
  isDelete: (e) => pl(e, Ee.Delete),
  model: (e) => e ? Ne(e.inherits, (t) => Ee.AnyRead.indexOf(t.name) >= 0) ? e.inherits?.genericArgs[0] : e.implements?.find((t) => Ee.AnyWrite.indexOf(t.name) >= 0)?.genericArgs[0] : null
};
function qa(e) {
  return e.input?.type || Il(bn(e));
}
function bs(e) {
  return e.endsWith("?") ? ba(e, 1) : e;
}
function Il(e) {
  return Ea[bs(e)];
}
function _a(e) {
  return e && Na[e] || "String";
}
function bn(e) {
  return e.type === "Nullable`1" ? e.genericArgs[0] : e.type;
}
function ln(e) {
  return e && Il(e) == "number" || !1;
}
function ws(e) {
  return e && e.toLowerCase() == "string" || !1;
}
function Qa(e) {
  return e == "List`1" || e.startsWith("List<") || e.endsWith("[]");
}
function xs(e) {
  if (!e?.type) return !1;
  const t = bn(e);
  return e.isValueType && t.indexOf("`") == -1 || e.isEnum ? !1 : Il(e.type) == null;
}
function ks(e) {
  if (!e?.type) return !1;
  const t = bn(e);
  if (e.isValueType && t.indexOf("`") == -1 || e.isEnum) return !0;
  const l = e.input?.type;
  return l && (l == "hidden" || l == "file" || l == "tag" || l == "combobox" || q.components?.[l]) ? !0 : Il(e.type) != null;
}
function sl(e, t) {
  let l = typeof e == "string" ? Dl(e) : e;
  l || (console.warn(`Metadata not found for: ${e}`), l = { request: { name: e } });
  let s = (
    /** @class */
    /* @__PURE__ */ (function() {
      return function(r) {
        Object.assign(this, r);
      };
    })()
  ), o = (
    /** @class */
    (function() {
      function r(u) {
        Object.assign(this, u);
      }
      return r.prototype.createResponse = function() {
        return l.returnsVoid ? void 0 : new s();
      }, r.prototype.getTypeName = function() {
        return l.request.name;
      }, r.prototype.getMethod = function() {
        return l.method || "POST";
      }, r;
    })()
  );
  return new o(t);
}
function Ka(e, t, l = {}) {
  let s = (
    /** @class */
    /* @__PURE__ */ (function() {
      return function(r) {
        Object.assign(this, r);
      };
    })()
  ), o = (
    /** @class */
    (function() {
      function r(u) {
        Object.assign(this, u);
      }
      return r.prototype.createResponse = function() {
        return typeof l.createResponse == "function" ? l.createResponse() : new s();
      }, r.prototype.getTypeName = function() {
        return e;
      }, r.prototype.getMethod = function() {
        return l.method || "POST";
      }, r;
    })()
  );
  return new o(t);
}
function ll(e, t) {
  return e ? (Object.keys(e).forEach((l) => {
    let s = e[l];
    typeof s == "string" ? s.startsWith("/Date") && (e[l] = jl(gt(s))) : s != null && typeof s == "object" && (Array.isArray(s) ? e[l] = Array.from(s) : e[l] = Object.assign({}, s));
  }), e) : {};
}
function Za(e, t) {
  let l = {};
  return Array.from(e.elements).forEach((s) => {
    let o = s;
    if (!o.id || o.value == null || o.value === "") return;
    const r = o.id.toLowerCase(), u = t && t.find((y) => y.name.toLowerCase() == r);
    let d = u?.type, f = u?.genericArgs?.[0], v = o.type === "checkbox" ? o.checked : o.value;
    ln(d) ? v = Number(v) : d === "List`1" && typeof v == "string" && (v = v.split(",").map((y) => ln(f) ? Number(y) : y)), l[o.id] = v;
  }), l;
}
function wn(e) {
  return e?.api?.operations && e.api.operations.length > 0;
}
function Wa(e) {
  if (!xn() && e?.assert && !q.metadata.value)
    throw new Error("useMetadata() not configured, see: https://docs.servicestack.net/vue/use-metadata");
  return q.metadata.value;
}
function al(e) {
  return e && wn(e) ? (e.date = ha(/* @__PURE__ */ new Date()), q.metadata.value = e, typeof localStorage < "u" && localStorage.setItem(Kt, JSON.stringify(e)), !0) : !1;
}
function Ga() {
  q.metadata.value = null, typeof localStorage < "u" && localStorage.removeItem(Kt);
}
function xn() {
  if (q.metadata.value != null) return !0;
  let e = globalThis.Server;
  if (wn(e))
    al(e);
  else {
    const t = typeof localStorage < "u" ? localStorage.getItem(Kt) : null;
    if (t)
      try {
        al(JSON.parse(t));
      } catch {
        console.error(`Could not JSON.parse ${Kt} from localStorage`);
      }
  }
  return q.metadata.value != null;
}
async function Yn(e, t) {
  let l = t ? await t() : await fetch(e);
  if (l.ok) {
    let s = await l.text();
    al(JSON.parse(s));
  } else
    console.error(`Could not download ${t ? "AppMetadata" : e}: ${l.statusText}`);
  wn(q.metadata.value) || console.warn("AppMetadata is not available");
}
async function Ja(e) {
  const { olderThan: t, resolvePath: l, resolve: s } = e || {};
  let o = xn() && t !== 0;
  if (o && t) {
    let r = gt(q.metadata.value?.date);
    (!r || (/* @__PURE__ */ new Date()).getTime() - r.getTime() > t) && (o = !1);
  }
  if (!o) {
    if ((l || s) && (await Yn(l || Kt, s), q.metadata.value != null))
      return;
    const r = ze("client");
    if (r != null) {
      const u = await r.api(new Ra());
      u.succeeded && al(u.response);
    }
    if (q.metadata.value != null) return;
    await Yn(Kt);
  }
  return q.metadata.value;
}
function it(e, t) {
  if (q.config.typeResolver) {
    let u = q.config.typeResolver(e, t);
    if (u) return u;
  }
  let l = q.metadata.value?.api;
  if (!l || !e) return null;
  let s = l.types.find((u) => u.name.toLowerCase() === e.toLowerCase() && (!t || u.namespace == t));
  if (s) return s;
  let o = Dl(e);
  if (o) return o.request;
  let r = l.operations.find((u) => u.response && u.response.name.toLowerCase() === e.toLowerCase() && (!t || u.response.namespace == t));
  return r ? r.response : null;
}
function Dl(e) {
  if (q.config.apiResolver) {
    const s = q.config.apiResolver(e);
    if (s) return s;
  }
  let t = q.metadata.value?.api;
  return t ? t.operations.find((s) => s.request.name.toLowerCase() === e.toLowerCase()) : null;
}
function Xa({ dataModel: e }) {
  const t = q.metadata.value?.api;
  if (!t) return [];
  let l = t.operations;
  if (e) {
    const s = typeof e == "string" ? it(e) : e;
    l = l.filter((o) => Cs(o.dataModel, s));
  }
  return l;
}
function kn(e) {
  return e ? it(e.name, e.namespace) : null;
}
function Cs(e, t) {
  return e && t && e.name === t.name && (!e.namespace || !t.namespace || e.namespace === t.namespace);
}
function Ya(e, t) {
  let l = it(e);
  return l && l.properties && l.properties.find((o) => o.name.toLowerCase() === t.toLowerCase());
}
function $s(e) {
  return Ls(it(e));
}
function Ls(e) {
  if (e && e.isEnum && e.enumNames != null) {
    let t = {};
    for (let l = 0; l < e.enumNames.length; l++) {
      const s = (e.enumDescriptions ? e.enumDescriptions[l] : null) || e.enumNames[l], o = (e.enumValues != null ? e.enumValues[l] : null) || e.enumNames[l];
      t[o] = s;
    }
    return t;
  }
  return null;
}
function Vs(e) {
  if (!e) return null;
  let t = {}, l = e.input && e.input.allowableEntries;
  if (l) {
    for (let o = 0; o < l.length; o++) {
      let r = l[o];
      t[r.key] = r.value;
    }
    return t;
  }
  let s = e.allowableValues || (e.input ? e.input.allowableValues : null);
  if (s) {
    for (let o = 0; o < s.length; o++) {
      let r = s[o];
      t[r] = r;
    }
    return t;
  }
  if (e.isEnum) {
    const o = e.genericArgs && e.genericArgs.length == 1 ? e.genericArgs[0] : e.type, r = it(o);
    if (r)
      return Ls(r);
  }
  return null;
}
function Cn(e) {
  if (!e) return;
  const t = [];
  return Object.keys(e).forEach((l) => t.push({ key: l, value: e[l] })), t;
}
function eo(e, t) {
  const s = ((o, r) => Object.assign({
    id: o,
    name: o,
    type: r
  }, t))(e.name, t?.type || qa(e) || "text");
  return e.isEnum && (s.type = "select", s.allowableEntries = Cn(Vs(e))), s;
}
function to(e) {
  let t = [];
  if (e) {
    const l = Xe(e), s = Dl(e.name), o = kn(s?.dataModel);
    l.forEach((r) => {
      if (!ks(r)) return;
      const u = eo(r, r.input);
      if (u.id = ya(u.id), u.type == "file" && r.uploadTo && !u.accept) {
        const d = q.metadata.value?.plugins.filesUpload?.locations.find((f) => f.name == r.uploadTo);
        d && !u.accept && d.allowExtensions && (u.accept = d.allowExtensions.map((f) => f.startsWith(".") ? f : `.${f}`).join(","));
      }
      if (o) {
        const d = o.properties?.find((f) => f.name == r.name);
        r.ref || (r.ref = d?.ref);
      }
      if (u.options)
        try {
          const d = {
            input: u,
            $typeFields: l.map((v) => v.name),
            $dataModelFields: o ? Xe(o).map((v) => v.name) : [],
            ...q.config.scopeWhitelist
          }, f = Hl(u.options, d);
          Object.keys(f).forEach((v) => {
            u[v] = f[v];
          });
        } catch {
          console.error(`failed to evaluate '${u.options}'`);
        }
      t.push(u);
    });
  }
  return t;
}
function $n(e, t) {
  if (!t.type)
    return console.error("enumDescriptions missing {type:'EnumType'} options"), [`${e}`];
  const l = it(t.type);
  if (!l?.enumValues)
    return console.error(`Could not find metadata for ${t.type}`), [`${e}`];
  const s = [];
  for (let o = 0; o < l.enumValues.length; o++) {
    const r = parseInt(l.enumValues[o]);
    r > 0 && (r & e) === r && s.push(l.enumDescriptions?.[o] || l.enumNames?.[o] || `${e}`);
  }
  return s;
}
function Ss(e) {
  return (t) => typeof t == "number" ? $n(t, { type: e }) : t;
}
function Xe(e) {
  if (!e) return [];
  let t = [], l = {};
  function s(o) {
    o.forEach((r) => {
      l[r.name] || (l[r.name] = 1, t.push(r));
    });
  }
  for (; e; )
    e.properties && s(e.properties), e = e.inherits ? kn(e.inherits) : null;
  return t.map((o) => o.type.endsWith("[]") ? { ...o, type: "List`1", genericArgs: [o.type.substring(0, o.type.length - 2)] } : o);
}
function pl(e, t) {
  return e.request.implements?.some((l) => l.name === t) || !1;
}
function Wt(e) {
  return e ? Ms(e, Xe(e)) : null;
}
function Ms(e, t) {
  let l = t.find((r) => r.name.toLowerCase() === "id");
  if (l && l.isPrimaryKey) return l;
  let o = t.find((r) => r.isPrimaryKey) || l;
  if (!o) {
    let r = Ee.model(e);
    if (r)
      return Ne(it(r), (u) => Wt(u));
    console.error(`Primary Key not found in ${e.name}`);
  }
  return o || null;
}
function lo(e, t) {
  return Ne(Wt(e), (l) => be(t, l.name));
}
function Ts(e, t, l) {
  return e && e.valueType === "none" ? "" : l.key === "%In" || l.key === "%Between" ? `(${l.value})` : no(t, l.value);
}
function no(e, t) {
  return e ? (e = bs(e), ln(e) || e === "Boolean" ? t : Qa(e) ? `[${t}]` : `'${t}'`) : t;
}
function bt(e, t) {
  return { name: e, value: t };
}
const so = [
  bt("=", "%"),
  bt("!=", "%!"),
  bt(">=", ">%"),
  bt(">", "%>"),
  bt("<=", "%<"),
  bt("<", "<%"),
  bt("In", "%In"),
  bt("Between", "%Between"),
  { name: "Starts With", value: "%StartsWith", types: "string" },
  { name: "Contains", value: "%Contains", types: "string" },
  { name: "Ends With", value: "%EndsWith", types: "string" },
  { name: "Exists", value: "%IsNotNull", valueType: "none" },
  { name: "Not Exists", value: "%IsNull", valueType: "none" }
];
function lt() {
  const e = c(() => q.metadata.value?.app || null), t = c(() => q.metadata.value?.api || null), l = c(() => q.metadata.value?.plugins?.autoQuery?.viewerConventions || so);
  return xn(), {
    loadMetadata: Ja,
    getMetadata: Wa,
    setMetadata: al,
    clearMetadata: Ga,
    metadataApp: e,
    metadataApi: t,
    filterDefinitions: l,
    typeOf: it,
    typeOfRef: kn,
    typeEquals: Cs,
    apiOf: Dl,
    findApis: Xa,
    typeName: Ua,
    typeName2: hs,
    property: Ya,
    enumOptions: $s,
    propertyOptions: Vs,
    createFormLayout: to,
    typeProperties: Xe,
    supportsProp: ks,
    Crud: Ee,
    Apis: Ot,
    getPrimaryKey: Wt,
    getPrimaryKeyByProps: Ms,
    getId: lo,
    createDto: sl,
    makeDto: Ka,
    toFormValues: ll,
    formValues: Za,
    isComplexProp: xs,
    asKvps: Cn,
    expandEnumFlags: $n,
    enumFlagsConverter: Ss
  };
}
class Ze {
  static Lookup = {};
  static async getOrFetchValue(t, l, s, o, r, u, d) {
    const f = Ze.getValue(s, d, r);
    return f ?? (await Ze.fetchLookupIds(t, l, s, o, r, u, [d]), Ze.getValue(s, d, r));
  }
  static getValue(t, l, s) {
    const o = Ze.Lookup[t];
    if (o) {
      const r = o[l];
      if (r)
        return s = s.toLowerCase(), r[s];
    }
  }
  static setValue(t, l, s, o) {
    const r = Ze.Lookup[t] ?? (Ze.Lookup[t] = {}), u = r[l] ?? (r[l] = {});
    s = s.toLowerCase(), u[s] = o;
  }
  static setRefValue(t, l) {
    const s = be(l, t.refId);
    if (s == null || t.refLabel == null)
      return null;
    const o = be(l, t.refLabel);
    return Ze.setValue(t.model, s, t.refLabel, o), o;
  }
  static async fetchLookupIds(t, l, s, o, r, u, d) {
    const f = l.operations.find((v) => Ee.isAnyQuery(v) && v.dataModel?.name == s);
    if (f) {
      const v = Ze.Lookup[s] ?? (Ze.Lookup[s] = {}), y = [];
      Object.keys(v).forEach((A) => {
        const B = v[A];
        be(B, r) && y.push(A);
      });
      const L = d.filter((A) => !y.includes(A));
      if (L.length == 0)
        return;
      const w = u ? null : `${o},${r}`, g = {
        [o + "In"]: L.join(",")
      };
      w && (g.fields = w);
      const h = sl(f, g), b = await t.api(h, { jsconfig: "edv,eccn" });
      if (b.succeeded)
        (be(b.response, "results") || []).forEach((B) => {
          if (!be(B, o)) {
            console.error(`result[${o}] == null`, B);
            return;
          }
          const le = `${be(B, o)}`, E = be(B, r);
          r = r.toLowerCase();
          const I = v[le] ?? (v[le] = {});
          I[r] = `${E}`;
        });
      else {
        console.error(`Failed to call ${f.request.name}`);
        return;
      }
    }
  }
}
let nn = () => (/* @__PURE__ */ new Date()).getTime(), ao = ["/", "T", ":", "-"], dt = {
  //locale: null,
  assumeUtc: !0,
  //number: null,
  date: {
    method: "Intl.DateTimeFormat",
    options: "{dateStyle:'medium'}"
  },
  maxFieldLength: 150,
  maxNestedFields: 2,
  maxNestedFieldLength: 30
}, oo = new Intl.RelativeTimeFormat(dt.locale, {}), es = 1440 * 60 * 1e3 * 365, Zl = {
  year: es,
  month: es / 12,
  day: 1440 * 60 * 1e3,
  hour: 3600 * 1e3,
  minute: 60 * 1e3,
  second: 1e3
}, Mt = {
  currency: As,
  bytes: Is,
  link: Ds,
  linkTel: Os,
  linkMailTo: Ps,
  icon: js,
  iconRounded: Bs,
  attachment: Hs,
  hidden: Rs,
  time: Es,
  relativeTime: Vn,
  relativeTimeFromMs: Ol,
  enumFlags: zs,
  formatDate: Gt,
  formatNumber: Ln
};
"iconOnError" in globalThis || (globalThis.iconOnError = Al);
class io {
  static currency = { method: "currency" };
  static bytes = { method: "bytes" };
  static link = { method: "link" };
  static linkTel = { method: "linkTel" };
  static linkMailTo = { method: "linkMailTo" };
  static icon = { method: "icon" };
  static iconRounded = { method: "iconRounded" };
  static attachment = { method: "attachment" };
  static time = { method: "time" };
  static relativeTime = { method: "relativeTime" };
  static relativeTimeFromMs = { method: "relativeTimeFromMs" };
  static date = { method: "formatDate" };
  static number = { method: "formatNumber" };
  static hidden = { method: "hidden" };
  static enumFlags = { method: "enumFlags" };
}
function ro(e) {
  dt = Object.assign({}, dt, e);
}
function uo(e) {
  Object.keys(e || {}).forEach((t) => {
    typeof e[t] == "function" && (Mt[t] = e[t]);
  });
}
function Fs() {
  return Mt;
}
function dl(e, t) {
  return t ? ct("span", e, t) : e;
}
function As(e, t) {
  const l = ft(t, ["currency"]);
  return dl(new Intl.NumberFormat(void 0, { style: "currency", currency: t?.currency || "USD" }).format(e), l);
}
function Is(e, t) {
  return dl(yn(e), t);
}
function Ds(e, t) {
  return ct("a", e, Bl({ ...t, href: e }));
}
function Os(e, t) {
  return ct("a", e, Bl({ ...t, href: `tel:${e}` }));
}
function Ps(e, t) {
  t || (t = {});
  let { subject: l, body: s } = t, o = ft(t, ["subject", "body"]), r = {};
  return l && (r.subject = l), s && (r.body = s), ct("a", e, Bl({ ...o, href: `mailto:${Qt(e, r)}` }));
}
function js(e, t) {
  return ct("img", void 0, Object.assign({ class: "w-6 h-6", title: e, src: _t(e), onerror: "iconOnError(this)" }, t));
}
function Bs(e, t) {
  return ct("img", void 0, Object.assign({ class: "w-8 h-8 rounded-full", title: e, src: _t(e), onerror: "iconOnError(this)" }, t));
}
function Hs(e, t) {
  let l = mn(e), o = ul(l) == null || pn(e) ? _t(e) : hn(e);
  const r = _t(o);
  let u = t && (t["icon-class"] || t.iconClass), d = ct("img", void 0, Object.assign({ class: "w-6 h-6", src: r, onerror: "iconOnError(this,'att')" }, u ? { class: u } : null)), f = `<span class="pl-1">${l}</span>`;
  return ct("a", d + f, Object.assign({ class: "flex", href: _t(e), title: e }, t ? ft(t, ["icon-class", "iconClass"]) : null));
}
function Rs(e) {
  return "";
}
function Es(e, t) {
  let l = typeof e == "string" ? new Date(rs(e) * 1e3) : Fl(e) ? gt(e) : null;
  return dl(l ? xa(l) : e, t);
}
function Gt(e, t) {
  if (e == null) return "";
  let l = typeof e == "number" ? new Date(e) : typeof e == "string" ? gt(e) : e;
  if (!Fl(l))
    return console.warn(`${l} is not a Date value`), e == null ? "" : `${e}`;
  let s = dt.date ? Pl(dt.date) : null;
  return dl(typeof s == "function" ? s(l) : wa(l), t);
}
function Ln(e, t) {
  if (typeof e != "number") return e;
  let l = dt.number ? Pl(dt.number) : null, s = typeof l == "function" ? l(e) : `${e}`;
  return s === "" && (console.warn(`formatNumber(${e}) => ${s}`, l), s = `${e}`), dl(s, t);
}
function wl(e) {
  const t = Math.floor(e / 1e3), l = Math.floor(t / 60), s = Math.floor(l / 60), o = Math.floor(s / 24);
  return o > 0 ? `${o}d ${wl(e - o * 24 * 60 * 6e4)}` : s > 0 ? `${s}h ${wl(e - s * 60 * 6e4)}` : l > 0 ? `${l}m ${wl(e - l * 6e4)}` : t > 0 ? `${t}s` : `${e}ms`;
}
function co(e) {
  return e >= 1e9 ? (e / 1e9).toFixed(1) + "b" : e >= 1e6 ? (e / 1e6).toFixed(1) + "m" : e >= 1e3 ? (e / 1e3).toFixed(1) + "k" : e.toLocaleString();
}
function Ns(e, t, l) {
  let s = Ca(e), o = t ? Pl(t) : null;
  if (typeof o == "function") {
    let u = l;
    if (t?.options)
      try {
        u = Hl(t.options, l);
      } catch (d) {
        console.error(`Could not evaluate '${t.options}'`, d, ", with scope:", l);
      }
    return o(e, u);
  }
  let r = s != null ? Fl(s) ? Gt(s, l) : typeof s == "number" ? Ln(s, l) : s : null;
  return r ?? "";
}
function ol(e, t, l) {
  return Pt(e) ? Ns(e, t, l) : po(e, t, l);
}
function fo(e) {
  if (e == null) return NaN;
  if (typeof e == "number")
    return e;
  if (Fl(e))
    return e.getTime() - nn();
  if (typeof e == "string") {
    let t = Number(e);
    if (!isNaN(t))
      return t;
    if (e[0] === "P" || e.startsWith("-P"))
      return rs(e) * 1e3 * -1;
    if (ka(e, ao) >= 0)
      return gt(e).getTime() - nn();
  }
  return NaN;
}
function Ol(e, t) {
  for (let l in Zl)
    if (Math.abs(e) > Zl[l] || l === "second")
      return (t || oo).format(Math.round(e / Zl[l]), l);
}
function Vn(e, t) {
  let l = fo(e);
  return isNaN(l) ? "" : Ol(l, t);
}
function vo(e, t) {
  return Ol(e.getTime() - (t ? t.getTime() : nn()));
}
function zs(e, t) {
  return $n(e, t).join(", ");
}
function Pl(e) {
  if (!e) return null;
  let { method: t, options: l } = e, s = `${t}(${l})`, o = Mt[s] || Mt[t];
  if (typeof o == "function") return o;
  let r = e.locale || dt.locale;
  if (t.startsWith("Intl.")) {
    let u = r ? `'${r}'` : "undefined", d = `return new ${t}(${u},${l || "undefined"})`;
    try {
      let f = Function(d)();
      return o = t === "Intl.DateTimeFormat" ? (v) => f.format(gt(v)) : t === "Intl.NumberFormat" ? (v) => f.format(Number(v)) : t === "Intl.RelativeTimeFormat" ? (v) => Vn(v, f) : (v) => f.format(v), Mt[s] = o;
    } catch (f) {
      console.error(`Invalid format: ${d}`, f);
    }
  } else {
    let u = globalThis[t];
    if (typeof u == "function") {
      let d = l != null ? Function("return " + l)() : void 0;
      return o = (f) => u(f, d, r), Mt[s] = o;
    }
    console.error(`No '${t}' function exists`, Object.keys(Mt));
  }
  return null;
}
function Us(e, t) {
  return e ? e.length > t ? e.substring(0, t) + "..." : e : "";
}
function qs(e) {
  return e.substring(0, 6) === "/Date(" ? Gt(gt(e)) : e;
}
function mo(e) {
  return Sn(Zt(e)).replace(/"/g, "");
}
function _s(e) {
  if (e == null || e === "") return "";
  if (typeof e == "string")
    try {
      return JSON.parse(e);
    } catch {
      console.warn("couldn't parse as JSON", e);
    }
  return e;
}
function Sn(e, t = 4) {
  return e = _s(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : JSON.stringify(e, void 0, t);
}
function go(e) {
  return e = _s(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : (e = Object.assign({}, e), e = Zt(e), Sn(e));
}
function Zt(e) {
  if (e == null) return null;
  if (typeof e == "string") return qs(e);
  if (Pt(e)) return e;
  if (e instanceof Date) return Gt(e);
  if (Array.isArray(e)) return e.map(Zt);
  if (typeof e == "object") {
    let t = {};
    return Object.keys(e).forEach((l) => {
      l != "__type" && (t[l] = Zt(e[l]));
    }), t;
  }
  return e;
}
function po(e, t, l) {
  let s = e;
  if (Array.isArray(e)) {
    if (Pt(e[0]))
      return s.join(",");
    e[0] != null && (s = e[0]);
  }
  if (s == null) return "";
  if (s instanceof Date) return Gt(s, l);
  let o = Object.keys(s), r = [];
  for (let u = 0; u < Math.min(dt.maxNestedFields, o.length); u++) {
    let d = o[u], f = `${Zt(s[d])}`;
    r.push(`<b class="font-medium">${d}</b>: ${Gl(Us(qs(f), dt.maxNestedFieldLength))}`);
  }
  return o.length > 2 && r.push("..."), ct("span", "{ " + r.join(", ") + " }", Object.assign({ title: Gl(mo(e)) }, l));
}
function Um() {
  return {
    Formats: io,
    setDefaultFormats: ro,
    getFormatters: Fs,
    setFormatters: uo,
    formatValue: ol,
    formatter: Pl,
    dateInputFormat: jl,
    currency: As,
    bytes: Is,
    link: Ds,
    linkTel: Os,
    linkMailTo: Ps,
    icon: js,
    iconRounded: Bs,
    attachment: Hs,
    hidden: Rs,
    time: Es,
    relativeTime: Vn,
    relativeTimeFromDate: vo,
    relativeTimeFromMs: Ol,
    enumFlags: zs,
    formatDate: Gt,
    formatNumber: Ln,
    humanifyMs: wl,
    humanifyNumber: co,
    indentJson: Sn,
    prettyJson: go,
    scrub: Zt,
    truncate: Us,
    apiValueFmt: Ns,
    iconOnError: Al
  };
}
const yo = ["title"], ho = /* @__PURE__ */ ie({
  __name: "RouterLink",
  props: {
    to: {}
  },
  setup(e) {
    const t = e, { config: l } = Bt(), s = () => l.value.navigate(t.to ?? "/");
    return (o, r) => (a(), i("a", ke({
      onClick: He(s, ["prevent"]),
      title: e.to,
      href: "javascript:void(0)"
    }, o.$attrs), [
      H(o.$slots, "default")
    ], 16, yo));
  }
});
class bo {
  callbacks = {};
  register(t, l) {
    this.callbacks[t] = l;
  }
  has(t) {
    return !!this.callbacks[t];
  }
  invoke(t, l) {
    const s = this.callbacks[t];
    typeof s == "function" && s(t, l);
  }
}
class wo {
  get length() {
    return typeof localStorage > "u" ? 0 : localStorage.length;
  }
  getItem(t) {
    return typeof localStorage > "u" ? null : localStorage.getItem(t);
  }
  setItem(t, l) {
    typeof localStorage > "u" || localStorage.setItem(t, l);
  }
  removeItem(t) {
    typeof localStorage > "u" || localStorage.removeItem(t);
  }
  clear() {
    typeof localStorage > "u" || localStorage.clear();
  }
  key(t) {
    return typeof localStorage > "u" ? null : localStorage.key(t);
  }
}
class q {
  static config = {
    redirectSignIn: "/signin",
    redirectSignOut: "/auth/logout",
    navigate: (t) => location.href = t,
    assetsPathResolver: (t) => t,
    fallbackPathResolver: (t) => t,
    storage: new wo(),
    tableIcon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><g fill='none' stroke='currentColor' stroke-width='1.5'><path d='M5 12v6s0 3 7 3s7-3 7-3v-6'/><path d='M5 6v6s0 3 7 3s7-3 7-3V6'/><path d='M12 3c7 0 7 3 7 3s0 3-7 3s-7-3-7-3s0-3 7-3Z'/></g></svg>" },
    scopeWhitelist: {
      enumFlagsConverter: Ss,
      ...Fs()
    }
  };
  static autoQueryGridDefaults = {
    deny: [],
    hide: [],
    toolbarButtonClass: void 0,
    tableStyle: "stripedRows",
    take: 25,
    maxFieldLength: 150
  };
  static events = $a();
  static user = T(null);
  static metadata = T(null);
  static components = {
    RouterLink: ho
  };
  static component(t) {
    const l = q.components[t];
    if (l) return l;
    const s = Wn(t), o = Object.keys(q.components).find((r) => Wn(r) === s);
    return o && q.components[o] || null;
  }
  static interceptors = new bo();
}
function xo(e) {
  q.config = Object.assign(q.config, e);
}
function ko(e) {
  q.autoQueryGridDefaults = Object.assign(q.autoQueryGridDefaults, e);
}
function Mn(e) {
  return e && q.config.assetsPathResolver ? q.config.assetsPathResolver(e) : e;
}
function Co(e) {
  return e && q.config.fallbackPathResolver ? q.config.fallbackPathResolver(e) : e;
}
function $o(e, t) {
  q.interceptors.register(e, t);
}
function Bt() {
  const e = c(() => q.config), t = c(() => q.autoQueryGridDefaults), l = q.events;
  return {
    config: e,
    setConfig: xo,
    events: l,
    autoQueryGridDefaults: t,
    setAutoQueryGridDefaults: ko,
    assetsPathResolver: Mn,
    fallbackPathResolver: Co,
    registerInterceptor: $o
  };
}
function jl(e) {
  if (e == null || typeof e == "object") return "";
  const t = gt(e);
  return t == null || t.toString() == "Invalid Date" ? "" : t.toISOString().substring(0, 10) ?? "";
}
function Qs(e) {
  if (e == null || typeof e == "object") return "";
  const t = gt(e);
  return t == null || t.toString() == "Invalid Date" ? "" : t.toISOString().substring(0, 19) ?? "";
}
function Ks(e) {
  return e == null ? "" : Sa(e);
}
function sn(e, t) {
  return q.config.inputValue ? q.config.inputValue(e, t) : e === "date" ? jl(t) : e === "datetime-local" ? Qs(t) : e === "time" ? Ks(t) : e === "number" || e === "range" ? t == null ? "" : Number(t) : t;
}
function Zs(e, t) {
  e.value = null, At(() => e.value = t);
}
function Ut(e) {
  return Object.keys(e).forEach((t) => {
    const l = e[t];
    e[t] = ss(l) ? W(l) : l;
  }), e;
}
function wt(e, t, l) {
  l ? (t.value = e.entering.cls + " " + e.entering.from, setTimeout(() => t.value = e.entering.cls + " " + e.entering.to, 0)) : (t.value = e.leaving.cls + " " + e.leaving.from, setTimeout(() => t.value = e.leaving.cls + " " + e.leaving.to, 0));
}
function xl(e) {
  if (typeof document > "u") return;
  let t = e?.after || document.activeElement, l = t && t.form;
  if (l) {
    let s = ':not([disabled]):not([tabindex="-1"])', o = l.querySelectorAll(`a:not([disabled]), button${s}, input[type=text]${s}, [tabindex]${s}`), r = Array.prototype.filter.call(
      o,
      (d) => d.offsetWidth > 0 || d.offsetHeight > 0 || d === t
    ), u = r.indexOf(t);
    u > -1 && (r[u + 1] || r[0]).focus();
  }
}
function Ht(e) {
  if (!e) return null;
  if (typeof e == "string")
    return e;
  const t = typeof e == "function" ? new e() : typeof e == "object" ? e : null;
  if (!t)
    throw new Error(`Invalid DTO Type '${typeof e}'`);
  if (typeof t.getTypeName != "function")
    throw new Error(`${JSON.stringify(t)} is not a Request DTO`);
  const l = t.getTypeName();
  if (!l)
    throw new Error("DTO Required");
  return l;
}
function ct(e, t, l) {
  l || (l = {});
  let s = l.cls || l.className || l.class;
  return s && (l = ft(l, ["cls", "class", "className"]), l.class = s), t == null ? `<${e}` + an(l) + "/>" : `<${e}` + an(l) + `>${t || ""}</${e}>`;
}
function an(e) {
  return Object.keys(e).reduce((t, l) => `${t} ${l}="${Gl(e[l])}"`, "");
}
function Bl(e) {
  return Object.assign({ target: "_blank", rel: "noopener", class: "text-blue-600" }, e);
}
function _t(e) {
  return Mn(e);
}
let Lo = ["string", "number", "boolean", "null", "undefined"];
function Pt(e) {
  return Lo.indexOf(typeof e) >= 0 || e instanceof Date;
}
function il(e) {
  return !Pt(e);
}
function Ll(e) {
  return typeof e == "string" ? JSON.parse(e) : null;
}
function Tn(e, t) {
  if (typeof history < "u") {
    const l = t ? Qt(us(location.href, "?"), e) : Va(location.href, e);
    history.pushState({}, "", l);
  }
}
function Hl(e, t) {
  if (["function", "Function", "eval", "=>", ";"].some((o) => e.includes(o)))
    throw new Error(`Unsafe script: '${e}'`);
  const s = Object.assign(
    Object.keys(globalThis).reduce((o, r) => (o[r] = void 0, o), {}),
    t
  );
  return new Function("with(this) { return (" + e + ") }").call(s);
}
function on(e) {
  typeof navigator < "u" && navigator.clipboard.writeText(e);
}
function Fn(e) {
  const t = q.config.storage.getItem(e);
  return t ? JSON.parse(t) : null;
}
function Rl(e, t) {
  return Qt(`swr.${La(e)}`, t ? Object.assign({}, e, t) : e);
}
function Vo(e) {
  if (e.request) {
    const t = Rl(e.request, e.args);
    q.config.storage.removeItem(t);
  }
}
async function Ws(e, t, l, s, o) {
  const r = Rl(t, s);
  l(new We({ response: Fn(r) }));
  const u = await e.api(t, s, o);
  if (u.succeeded && u.response) {
    u.response._date = (/* @__PURE__ */ new Date()).valueOf();
    const d = JSON.stringify(u.response);
    q.config.storage.setItem(r, d), l(u);
  }
  return u;
}
function Gs(e, t) {
  let l = null;
  return (...s) => {
    l && clearTimeout(l), l = setTimeout(() => {
      e(...s);
    }, t || 100);
  };
}
function Tt(e) {
  return typeof e == "string" ? e.split(",") : e || [];
}
function Ft(e, t) {
  const l = Tt(t);
  return e.reduce((s, o) => (s[o] = !l.includes(o), s), {});
}
function So(e) {
  return new Promise((t) => setTimeout(t, e));
}
function Js(e) {
  const t = [], l = [];
  for (const s of e) {
    const o = s.toLowerCase();
    l.includes(o) || (t.push(s), l.push(o));
  }
  return t;
}
function Xs() {
  return {
    dateInputFormat: jl,
    dateTimeInputFormat: Qs,
    timeInputFormat: Ks,
    textInputValue: sn,
    setRef: Zs,
    unRefs: Ut,
    transition: wt,
    focusNextElement: xl,
    getTypeName: Ht,
    htmlTag: ct,
    htmlAttrs: an,
    linkAttrs: Bl,
    toAppUrl: _t,
    isPrimitive: Pt,
    isComplexType: il,
    pushState: Tn,
    scopedExpr: Hl,
    copyText: on,
    fromCache: Fn,
    swrCacheKey: Rl,
    swrClear: Vo,
    swrApi: Ws,
    asStrings: Tt,
    asOptions: Ft,
    createDebounce: Gs,
    delay: So,
    uniqueIgnoreCase: Js
  };
}
function cl(e) {
  const t = T(!1), l = T(), s = T(), o = e ?? ze("client");
  function r({ message: h, errorCode: b, fieldName: A, errors: B }) {
    return b || (b = "Exception"), B || (B = []), l.value = A ? new Kl({
      errorCode: b,
      message: h,
      errors: [new Gn({ fieldName: A, errorCode: b, message: h })]
    }) : new Kl({ errorCode: b, message: h, errors: B });
  }
  function u({ fieldName: h, message: b, errorCode: A }) {
    if (A || (A = "Exception"), !l.value)
      r({ fieldName: h, message: b, errorCode: A });
    else {
      let B = new Kl(l.value);
      B.errors = [
        ...(B.errors || []).filter((le) => le.fieldName?.toLowerCase() !== h?.toLowerCase()),
        new Gn({ fieldName: h, message: b, errorCode: A })
      ], l.value = B;
    }
  }
  async function d(h, b, A) {
    t.value = !0;
    let B = await o.api(Ut(h), b, A);
    return t.value = !1, s.value = B.response, l.value = B.error, B;
  }
  async function f(h, b, A) {
    t.value = !0;
    let B = await o.apiVoid(Ut(h), b, A);
    return t.value = !1, s.value = B.response, l.value = B.error, B;
  }
  async function v(h, b, A, B) {
    t.value = !0;
    let le = await o.apiForm(Ut(h), b, A, B);
    return t.value = !1, s.value = le.response, l.value = le.error, le;
  }
  async function y(h, b, A, B) {
    t.value = !0;
    let le = await o.apiFormVoid(Ut(h), b, A, B);
    return t.value = !1, s.value = le.response, l.value = le.error, le;
  }
  async function L(h, b, A, B) {
    return Ws(o, h, b, A, B);
  }
  function w(h, b) {
    const A = T(new We()), B = Gs(async (le) => {
      A.value = await o.api(le);
    }, b?.delayMs);
    return kl(async () => {
      const le = h(), E = Fn(Rl(le));
      E && (A.value = new We({ response: E })), b?.delayMs === 0 ? A.value = await o.api(le) : B(le);
    }), (async () => A.value = await o.api(h(), b?.args, b?.method))(), A;
  }
  let g = { setError: r, addFieldError: u, loading: t, error: l, api: d, apiVoid: f, apiForm: v, apiFormVoid: y, swr: L, swrEffect: w, unRefs: Ut, setRef: Zs };
  return rl("ApiState", g), g;
}
function Ys(e) {
  return e && e.SessionId ? Ma(e) : e;
}
function Mo(e) {
  q.user.value = Ys(e), q.events.publish("signIn", e);
}
function To() {
  q.user.value = null, q.events.publish("signOut", null);
}
const An = (e) => e?.roles || [], In = (e) => e?.permissions || [];
function ea(e) {
  return An(q.user.value).indexOf(e) >= 0;
}
function Fo(e) {
  return In(q.user.value).indexOf(e) >= 0;
}
function Dn() {
  return ea("Admin");
}
function nl(e) {
  if (!e) return !1;
  if (!e.requiresAuth)
    return !0;
  const t = q.user.value;
  if (!t)
    return !1;
  if (Dn())
    return !0;
  let [l, s] = [An(t), In(t)], [o, r, u, d] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ];
  return !(!o.every((f) => l.indexOf(f) >= 0) || u.length > 0 && !u.some((f) => l.indexOf(f) >= 0) || !r.every((f) => s.indexOf(f) >= 0) || d.length > 0 && !d.every((f) => s.indexOf(f) >= 0));
}
function Ao(e) {
  if (!e || !e.requiresAuth) return null;
  const t = q.user.value;
  if (!t)
    return `<b>${e.request.name}</b> requires Authentication`;
  if (Dn())
    return null;
  let [l, s] = [An(t), In(t)], [o, r, u, d] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ], f = o.filter((y) => l.indexOf(y) < 0);
  if (f.length > 0)
    return `Requires ${f.map((y) => "<b>" + y + "</b>").join(", ")} Role` + (f.length > 1 ? "s" : "");
  let v = r.filter((y) => s.indexOf(y) < 0);
  return v.length > 0 ? `Requires ${v.map((y) => "<b>" + y + "</b>").join(", ")} Permission` + (v.length > 1 ? "s" : "") : u.length > 0 && !u.some((y) => l.indexOf(y) >= 0) ? `Requires any ${u.filter((y) => l.indexOf(y) < 0).map((y) => "<b>" + y + "</b>").join(", ")} Role` + (f.length > 1 ? "s" : "") : d.length > 0 && !d.every((y) => s.indexOf(y) >= 0) ? `Requires any ${d.filter((y) => s.indexOf(y) < 0).map((y) => "<b>" + y + "</b>").join(", ")} Permission` + (v.length > 1 ? "s" : "") : null;
}
function On() {
  const e = c(() => q.user.value || null), t = c(() => q.user.value != null);
  return { signIn: Mo, signOut: To, user: e, toAuth: Ys, isAuthenticated: t, hasRole: ea, hasPermission: Fo, isAdmin: Dn, canAccess: nl, invalidAccessMessage: Ao };
}
function Ke(e, t) {
  return Array.isArray(e) ? e.indexOf(t) >= 0 : e == t || e.includes(t);
}
const Vl = {
  blue: "text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-200",
  purple: "text-purple-600 dark:text-purple-400 hover:text-purple-800 dark:hover:text-purple-200",
  red: "text-red-700 dark:text-red-400 hover:text-red-900 dark:hover:text-red-200",
  green: "text-green-600 dark:text-green-400 hover:text-green-800 dark:hover:text-green-200",
  sky: "text-sky-600 dark:text-sky-400 hover:text-sky-800 dark:hover:text-sky-200",
  cyan: "text-cyan-600 dark:text-cyan-400 hover:text-cyan-800 dark:hover:text-cyan-200",
  indigo: "text-indigo-600 dark:text-indigo-400 hover:text-indigo-800 dark:hover:text-indigo-200"
}, at = {
  base: "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 disabled:bg-slate-50 dark:disabled:bg-slate-900 disabled:text-slate-500 disabled:border-slate-200 dark:disabled:border-slate-700 disabled:shadow-none",
  invalid: "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500",
  valid: "shadow-sm focus:ring-indigo-500 focus:border-indigo-500 border-gray-300 dark:border-gray-600"
}, el = {
  panelClass: "shadow sm:rounded-md",
  formClass: "space-y-6 bg-white dark:bg-black py-6 px-4 sm:p-6",
  headingClass: "text-lg font-medium leading-6 text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400"
}, qt = {
  panelClass: "pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg",
  formClass: "flex h-full flex-col divide-y divide-gray-200 dark:divide-gray-700 shadow-xl bg-white dark:bg-black",
  titlebarClass: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6",
  headingClass: "text-lg font-medium text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400",
  closeButtonClass: "rounded-md bg-gray-50 dark:bg-gray-900 text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
}, rn = {
  modalClass: "relative transform overflow-hidden rounded-lg bg-white dark:bg-black text-left shadow-xl transition-all sm:my-8",
  sizeClass: "sm:max-w-prose lg:max-w-screen-md xl:max-w-screen-lg 2xl:max-w-screen-xl sm:w-full"
}, Be = {
  panelClass(e = "slideOver") {
    return e == "card" ? el.panelClass : qt.panelClass;
  },
  formClass(e = "slideOver") {
    return e == "card" ? el.formClass : qt.formClass;
  },
  headingClass(e = "slideOver") {
    return e == "card" ? el.headingClass : qt.headingClass;
  },
  subHeadingClass(e = "slideOver") {
    return e == "card" ? el.subHeadingClass : qt.subHeadingClass;
  },
  buttonsClass: "px-4 py-3 bg-gray-50 dark:bg-gray-900 sm:px-6 flex flex-wrap justify-between",
  legendClass: "text-base font-medium text-gray-900 dark:text-gray-100 text-center mb-4"
}, ge = {
  getGridClass(e = "stripedRows") {
    return ge.gridClass;
  },
  getGrid2Class(e = "stripedRows") {
    return Ke(e, "fullWidth") ? "overflow-x-auto" : ge.grid2Class;
  },
  getGrid3Class(e = "stripedRows") {
    return Ke(e, "fullWidth") ? "inline-block min-w-full py-2 align-middle" : ge.grid3Class;
  },
  getGrid4Class(e = "stripedRows") {
    return Ke(e, "whiteBackground") ? "" : Ke(e, "fullWidth") ? "overflow-hidden shadow-sm ring-1 ring-black/5" : ge.grid4Class;
  },
  getTableClass(e = "stripedRows") {
    return Ke(e, "fullWidth") || Ke(e, "verticalLines") ? "min-w-full divide-y divide-gray-300" : ge.tableClass;
  },
  getTheadClass(e = "stripedRows") {
    return Ke(e, "whiteBackground") ? "" : ge.theadClass;
  },
  getTheadRowClass(e = "stripedRows") {
    return ge.theadRowClass + (Ke(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
  },
  getTheadCellClass(e = "stripedRows") {
    return ge.theadCellClass + (Ke(e, "uppercaseHeadings") ? " uppercase" : "");
  },
  getTbodyClass(e = "stripedRows") {
    return (Ke(e, "whiteBackground") || Ke(e, "verticalLines") ? "divide-y divide-gray-200 dark:divide-gray-800" : ge.tableClass) + (Ke(e, "verticalLines") ? " bg-white" : "");
  },
  getTableRowClass(e = "stripedRows", t, l, s) {
    return (s ? "cursor-pointer " : "") + (l ? "bg-indigo-100 dark:bg-blue-800" : (s ? "hover:bg-yellow-50 dark:hover:bg-blue-900 " : "") + (Ke(e, "stripedRows") ? t % 2 == 0 ? "bg-white dark:bg-black" : "bg-gray-50 dark:bg-gray-800" : "bg-white dark:bg-black")) + (Ke(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
  },
  gridClass: "flex flex-col",
  //original -margins + padding forces scroll bars when parent is w-full for no clear benefits?
  //original: -my-2 -mx-4 overflow-x-auto sm:-mx-6 lg:-mx-8
  grid2Class: "",
  //original: inline-block min-w-full py-2 align-middle md:px-6 lg:px-8
  grid3Class: "inline-block min-w-full py-2 align-middle",
  grid4Class: "overflow-hidden shadow ring-1 ring-black/5 md:rounded-lg",
  tableClass: "min-w-full divide-y divide-gray-200 dark:divide-gray-700",
  theadClass: "bg-gray-50 dark:bg-gray-900",
  tableCellClass: "px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400",
  theadRowClass: "select-none",
  theadCellClass: "px-6 py-4 text-left text-sm font-medium tracking-wider whitespace-nowrap",
  toolbarButtonClass: "inline-flex items-center px-2.5 py-1.5 border border-gray-300 dark:border-gray-700 shadow-sm text-sm font-medium rounded text-gray-700 dark:text-gray-300 bg-white dark:bg-black hover:bg-gray-50 dark:hover:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"
}, Io = {
  colspans: "col-span-3 sm:col-span-3"
};
function kt(e, t, l) {
  const s = e.filter((o) => o).join(" ");
  return l ??= q.config.filterInputClass == null ? void 0 : (o) => q.config.filterInputClass(o, t), l ? l(s) : s;
}
const qm = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  a: Vl,
  card: el,
  dummy: Io,
  filterClass: kt,
  form: Be,
  grid: ge,
  input: at,
  modal: rn,
  slideOver: qt
}, Symbol.toStringTag, { value: "Module" })), Do = { class: "flex items-center" }, Oo = {
  key: 0,
  class: "flex-shrink-0 mr-3"
}, Po = {
  key: 0,
  class: "h-5 w-5 text-yellow-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, jo = {
  key: 1,
  class: "h-5 w-5 text-red-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Bo = {
  key: 2,
  class: "h-5 w-5 text-blue-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Ho = {
  key: 3,
  class: "h-5 w-5 text-green-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Ro = /* @__PURE__ */ ie({
  __name: "Alert",
  props: {
    type: { default: "warn" },
    hideIcon: { type: Boolean }
  },
  setup(e) {
    const t = e, l = c(() => t.type == "info" ? "bg-blue-50 dark:bg-blue-200" : t.type == "error" ? "bg-red-50 dark:bg-red-200" : t.type == "success" ? "bg-green-50 dark:bg-green-200" : "bg-yellow-50 dark:bg-yellow-200"), s = c(() => t.type == "info" ? "border-blue-400" : t.type == "error" ? "border-red-400" : t.type == "success" ? "border-green-400" : "border-yellow-400"), o = c(() => t.type == "info" ? "text-blue-700" : t.type == "error" ? "text-red-700" : t.type == "success" ? "text-green-700" : "text-yellow-700");
    return (r, u) => (a(), i("div", {
      class: m([l.value, s.value, "border-l-4 p-4"])
    }, [
      n("div", Do, [
        e.hideIcon ? x("", !0) : (a(), i("div", Oo, [
          e.type == "warn" ? (a(), i("svg", Po, [...u[0] || (u[0] = [
            n("path", {
              "fill-rule": "evenodd",
              d: "M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "error" ? (a(), i("svg", jo, [...u[1] || (u[1] = [
            n("path", {
              "fill-rule": "evenodd",
              d: "M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "info" ? (a(), i("svg", Bo, [...u[2] || (u[2] = [
            n("path", {
              "fill-rule": "evenodd",
              d: "M19 10.5a8.5 8.5 0 11-17 0 8.5 8.5 0 0117 0zM8.25 9.75A.75.75 0 019 9h.253a1.75 1.75 0 011.709 2.13l-.46 2.066a.25.25 0 00.245.304H11a.75.75 0 010 1.5h-.253a1.75 1.75 0 01-1.709-2.13l.46-2.066a.25.25 0 00-.245-.304H9a.75.75 0 01-.75-.75zM10 7a1 1 0 100-2 1 1 0 000 2z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "success" ? (a(), i("svg", Ho, [...u[3] || (u[3] = [
            n("path", {
              "fill-rule": "evenodd",
              d: "M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : x("", !0)
        ])),
        n("div", null, [
          n("p", {
            class: m([o.value, "text-sm"])
          }, [
            H(r.$slots, "default")
          ], 2)
        ])
      ])
    ], 2));
  }
}), Eo = {
  key: 0,
  class: "rounded-md bg-green-50 dark:bg-green-200 p-4",
  role: "alert"
}, No = { class: "flex" }, zo = { class: "ml-3" }, Uo = { class: "text-sm font-medium text-green-800" }, qo = { key: 0 }, _o = { class: "ml-auto pl-3" }, Qo = { class: "-mx-1.5 -my-1.5" }, Ko = /* @__PURE__ */ ie({
  __name: "AlertSuccess",
  props: {
    message: {}
  },
  setup(e) {
    const t = T(!1);
    return (l, s) => t.value ? x("", !0) : (a(), i("div", Eo, [
      n("div", No, [
        s[2] || (s[2] = n("div", { class: "flex-shrink-0" }, [
          n("svg", {
            class: "h-5 w-5 text-green-400 dark:text-green-500",
            fill: "none",
            stroke: "currentColor",
            viewBox: "0 0 24 24",
            xmlns: "http://www.w3.org/2000/svg"
          }, [
            n("path", {
              "stroke-linecap": "round",
              "stroke-linejoin": "round",
              "stroke-width": "2",
              d: "M5 13l4 4L19 7"
            })
          ])
        ], -1)),
        n("div", zo, [
          n("h3", Uo, [
            e.message ? (a(), i("span", qo, M(e.message), 1)) : H(l.$slots, "default", { key: 1 })
          ])
        ]),
        n("div", _o, [
          n("div", Qo, [
            n("button", {
              type: "button",
              class: "inline-flex rounded-md bg-green-50 dark:bg-green-200 p-1.5 text-green-500 dark:text-green-600 hover:bg-green-100 dark:hover:bg-green-800 dark:hover:text-white focus:outline-none focus:ring-2 focus:ring-green-600 dark:focus:ring-green-300 focus:ring-offset-2 focus:ring-offset-green-50 dark:ring-offset-green-900",
              onClick: s[0] || (s[0] = (o) => t.value = !0)
            }, [...s[1] || (s[1] = [
              n("span", { class: "sr-only" }, "Dismiss", -1),
              n("svg", {
                class: "h-5 w-5",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                n("path", { d: "M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" })
              ], -1)
            ])])
          ])
        ])
      ])
    ]));
  }
}), Zo = { class: "flex" }, Wo = { class: "ml-3" }, Go = { class: "text-sm text-red-700 dark:text-red-200" }, Jo = /* @__PURE__ */ ie({
  __name: "ErrorSummary",
  props: {
    status: {},
    except: {},
    class: {}
  },
  setup(e) {
    const t = e;
    let l = ze("ApiState", void 0);
    const s = c(() => t.status || l?.error.value ? Ta.call({ responseStatus: t.status ?? l?.error.value }, t.except ?? []) : null);
    return (o, r) => s.value ? (a(), i("div", {
      key: 0,
      class: m(`bg-red-50 dark:bg-red-900 border-l-4 border-red-400 p-4 ${o.$props.class}`)
    }, [
      n("div", Zo, [
        r[0] || (r[0] = n("div", { class: "flex-shrink-0" }, [
          n("svg", {
            class: "h-5 w-5 text-red-400",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 24 24"
          }, [
            n("path", {
              fill: "currentColor",
              d: "M12 2c5.53 0 10 4.47 10 10s-4.47 10-10 10S2 17.53 2 12S6.47 2 12 2m3.59 5L12 10.59L8.41 7L7 8.41L10.59 12L7 15.59L8.41 17L12 13.41L15.59 17L17 15.59L13.41 12L17 8.41L15.59 7Z"
            })
          ])
        ], -1)),
        n("div", Wo, [
          n("p", Go, M(s.value), 1)
        ])
      ])
    ], 2)) : x("", !0);
  }
}), Xo = ["id", "aria-describedby"], Yo = /* @__PURE__ */ ie({
  __name: "InputDescription",
  props: {
    id: {},
    description: {}
  },
  setup(e) {
    return (t, l) => e.description ? (a(), i("div", {
      key: "description",
      class: "mt-2 text-sm text-gray-500",
      id: `${e.id}-description`,
      "aria-describedby": `${e.id}-description`
    }, [
      n("div", null, M(e.description), 1)
    ], 8, Xo)) : x("", !0);
  }
}), ta = ie({
  inheritAttrs: !1,
  props: {
    image: Object,
    svg: String,
    src: String,
    alt: String,
    type: String
  },
  setup(e, { attrs: t }) {
    return () => {
      let l = e.image;
      if (e.type) {
        const { typeOf: r } = lt(), u = r(e.type);
        u || console.warn(`Type ${e.type} does not exist`), u?.icon ? l = u?.icon : console.warn(`Type ${e.type} does not have a [Svg] icon`);
      }
      let s = e.svg || l?.svg || "";
      if (s.startsWith("<svg ")) {
        let u = Tl(s, ">").indexOf("class="), d = `${l?.cls || ""} ${t.class || ""}`;
        if (u == -1)
          s = `<svg class="${d}" ${s.substring(4)}`;
        else {
          const f = u + 6 + 1;
          s = `${s.substring(0, f) + d} ${s.substring(f)}`;
        }
        return Vt("span", { innerHTML: s });
      } else
        return Vt("img", {
          class: [l?.cls, t.class],
          src: Mn(e.src || l?.uri),
          onError: (r) => Al(r.target)
        });
    };
  }
}), ei = { class: "text-2xl font-semibold text-gray-900 dark:text-gray-300" }, ti = { class: "flex" }, li = /* @__PURE__ */ ie({
  __name: "Loading",
  props: {
    imageClass: { default: "w-6 h-6" }
  },
  setup(e) {
    return (t, l) => (a(), i("div", ei, [
      n("div", ti, [
        (a(), i("svg", {
          class: m(["self-center inline mr-2 text-gray-200 animate-spin dark:text-gray-600 fill-gray-600 dark:fill-gray-300", e.imageClass]),
          role: "status",
          viewBox: "0 0 100 101",
          fill: "none",
          xmlns: "http://www.w3.org/2000/svg"
        }, [...l[0] || (l[0] = [
          n("path", {
            d: "M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z",
            fill: "currentColor"
          }, null, -1),
          n("path", {
            d: "M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z",
            fill: "currentFill"
          }, null, -1)
        ])], 2)),
        n("span", null, [
          H(t.$slots, "default")
        ])
      ])
    ]));
  }
}), ni = ["href", "onClick"], si = ["type"], ts = "inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 shadow-sm text-sm font-medium rounded-md text-gray-700 dark:text-gray-200 disabled:text-gray-400 bg-white dark:bg-black hover:bg-gray-50 hover:dark:bg-gray-900 disabled:hover:bg-white dark:disabled:hover:bg-black focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black", ai = /* @__PURE__ */ ie({
  __name: "OutlineButton",
  props: {
    type: { default: "submit" },
    href: {}
  },
  setup(e) {
    return (t, l) => {
      const s = z("router-link");
      return e.href ? (a(), X(s, {
        key: 0,
        to: e.href
      }, {
        default: xe(({ navigate: o }) => [
          n("button", {
            class: m(ts),
            href: e.href,
            onClick: o
          }, [
            H(t.$slots, "default")
          ], 8, ni)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", ke({
        key: 1,
        type: e.type,
        class: ts
      }, t.$attrs), [
        H(t.$slots, "default")
      ], 16, si));
    };
  }
}), oi = ["href", "onClick"], ii = ["type"], ri = /* @__PURE__ */ ie({
  __name: "PrimaryButton",
  props: {
    type: { default: "submit" },
    href: {},
    color: { default: "indigo" }
  },
  setup(e) {
    const t = e, l = {
      blue: "focus:ring-blue-500 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 disabled:hover:bg-blue-400 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800",
      purple: "focus:ring-purple-500 bg-purple-600 hover:bg-purple-700 disabled:bg-purple-400 disabled:hover:bg-purple-400 dark:bg-purple-600 dark:hover:bg-purple-700 dark:focus:ring-purple-800",
      red: "focus:ring-red-500 bg-red-600 hover:bg-red-700 disabled:bg-red-400 disabled:hover:bg-red-400 focus:ring-red-500 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-500",
      green: "focus:ring-green-500 bg-green-600 hover:bg-green-700 disabled:bg-green-400 disabled:hover:bg-green-400 focus:ring-green-500 dark:bg-green-600 dark:hover:bg-green-700 dark:focus:ring-green-500",
      sky: "focus:ring-sky-500 bg-sky-600 hover:bg-sky-700 disabled:bg-sky-400 disabled:hover:bg-sky-400 dark:bg-sky-600 dark:hover:bg-sky-700 dark:focus:ring-sky-500",
      cyan: "focus:ring-cyan-500 bg-cyan-600 hover:bg-cyan-700 disabled:bg-cyan-400 disabled:hover:bg-cyan-400 dark:bg-cyan-600 dark:hover:bg-cyan-700 dark:focus:ring-cyan-500",
      indigo: "focus:ring-indigo-500 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 disabled:hover:bg-indigo-400 dark:bg-indigo-600 dark:hover:bg-indigo-700 dark:focus:ring-indigo-800"
    }, s = c(() => "inline-flex justify-center rounded-md border border-transparent py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 dark:ring-offset-black text-white " + (l[t.color] || l.indigo));
    return (o, r) => {
      const u = z("router-link");
      return e.href ? (a(), X(u, {
        key: 0,
        to: e.href
      }, {
        default: xe(({ navigate: d }) => [
          n("button", {
            class: m(s.value),
            href: e.href,
            onClick: d
          }, [
            H(o.$slots, "default")
          ], 10, oi)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", ke({
        key: 1,
        type: e.type,
        class: s.value
      }, o.$attrs), [
        H(o.$slots, "default")
      ], 16, ii));
    };
  }
}), ui = ["type", "href", "onClick"], di = ["type"], ls = "inline-flex justify-center rounded-md border border-gray-300 py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-400 dark:hover:text-white hover:bg-gray-50 dark:hover:bg-gray-700 focus:ring-indigo-500 dark:focus:ring-indigo-600 dark:ring-offset-black", ci = /* @__PURE__ */ ie({
  __name: "SecondaryButton",
  props: {
    type: {},
    href: {}
  },
  setup(e) {
    return (t, l) => {
      const s = z("router-link");
      return e.href ? (a(), X(s, {
        key: 0,
        to: e.href
      }, {
        default: xe(({ navigate: o }) => [
          n("button", {
            type: e.type ?? "button",
            class: m(ls),
            href: e.href,
            onClick: o
          }, [
            H(t.$slots, "default")
          ], 8, ui)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", ke({
        key: 1,
        type: e.type ?? "button",
        class: ls
      }, t.$attrs), [
        H(t.$slots, "default")
      ], 16, di));
    };
  }
}), fi = /* @__PURE__ */ ie({
  __name: "TextLink",
  props: {
    color: { default: "blue" }
  },
  setup(e) {
    const t = fa(), l = e, s = c(() => (Vl[l.color] || Vl.blue) + (t.href ? "" : " cursor-pointer"));
    return (o, r) => (a(), i("a", {
      class: m(s.value)
    }, [
      H(o.$slots, "default")
    ], 2));
  }
}), vi = {
  class: "flex",
  "aria-label": "Breadcrumb"
}, mi = {
  role: "list",
  class: "flex items-center space-x-4"
}, gi = ["href", "title"], pi = { class: "sr-only" }, yi = /* @__PURE__ */ ie({
  __name: "Breadcrumbs",
  props: {
    homeHref: { default: "/" },
    homeLabel: { default: "Home" }
  },
  setup(e) {
    return (t, l) => (a(), i("nav", vi, [
      n("ol", mi, [
        n("li", null, [
          n("div", null, [
            n("a", {
              href: e.homeHref,
              class: "text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400",
              title: e.homeLabel
            }, [
              l[0] || (l[0] = n("svg", {
                class: "h-6 w-6 flex-shrink-0",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                n("path", {
                  "fill-rule": "evenodd",
                  d: "M9.293 2.293a1 1 0 011.414 0l7 7A1 1 0 0117 11h-1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-3a1 1 0 00-1-1H9a1 1 0 00-1 1v3a1 1 0 01-1 1H5a1 1 0 01-1-1v-6H3a1 1 0 01-.707-1.707l7-7z",
                  "clip-rule": "evenodd"
                })
              ], -1)),
              n("span", pi, M(e.homeLabel), 1)
            ], 8, gi)
          ])
        ]),
        H(t.$slots, "default")
      ])
    ]));
  }
}), hi = { class: "flex items-center" }, bi = ["href", "title"], wi = ["title"], xi = /* @__PURE__ */ ie({
  __name: "Breadcrumb",
  props: {
    href: {},
    title: {}
  },
  setup(e) {
    return (t, l) => (a(), i("li", null, [
      n("div", hi, [
        l[0] || (l[0] = n("svg", {
          class: "h-6 w-6 flex-shrink-0 text-gray-400 dark:text-gray-500",
          xmlns: "http://www.w3.org/2000/svg",
          viewBox: "0 0 20 20",
          fill: "currentColor",
          "aria-hidden": "true"
        }, [
          n("path", {
            "fill-rule": "evenodd",
            d: "M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z",
            "clip-rule": "evenodd"
          })
        ], -1)),
        e.href ? (a(), i("a", {
          key: 0,
          href: e.href,
          class: "ml-4 text-lg font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300",
          title: e.title
        }, [
          H(t.$slots, "default")
        ], 8, bi)) : (a(), i("span", {
          key: 1,
          class: "ml-4 text-lg font-medium text-gray-700 dark:text-gray-300",
          title: e.title
        }, [
          H(t.$slots, "default")
        ], 8, wi))
      ])
    ]));
  }
}), ki = {
  key: 0,
  class: "text-base font-semibold text-gray-500 dark:text-gray-400"
}, Ci = {
  role: "list",
  class: "mt-4 divide-y divide-gray-200 dark:divide-gray-800 border-t border-b border-gray-200 dark:border-gray-800"
}, $i = /* @__PURE__ */ ie({
  __name: "NavList",
  props: {
    title: {}
  },
  setup(e) {
    return (t, l) => (a(), i("div", null, [
      e.title ? (a(), i("h2", ki, M(e.title), 1)) : x("", !0),
      n("ul", Ci, [
        H(t.$slots, "default")
      ])
    ]));
  }
}), Li = { class: "relative flex items-start space-x-4 py-6" }, Vi = { class: "flex-shrink-0" }, Si = { class: "flex h-12 w-12 items-center justify-center rounded-lg bg-indigo-50 dark:bg-indigo-900" }, Mi = { class: "min-w-0 flex-1" }, Ti = { class: "text-base font-medium text-gray-900 dark:text-gray-100" }, Fi = { class: "rounded-sm focus-within:ring-2 focus-within:ring-indigo-500 focus-within:ring-offset-2" }, Ai = ["href"], Ii = { class: "text-base text-gray-500" }, Di = /* @__PURE__ */ ie({
  __name: "NavListItem",
  props: {
    title: {},
    href: {},
    icon: {},
    iconSvg: {},
    iconSrc: {},
    iconAlt: {}
  },
  setup(e) {
    return (t, l) => {
      const s = z("Icon");
      return a(), i("li", Li, [
        n("div", Vi, [
          n("span", Si, [
            he(s, {
              class: "w-6 h-6 text-indigo-700 dark:text-indigo-300",
              image: e.icon,
              src: e.iconSrc,
              svg: e.iconSvg,
              alt: e.iconAlt
            }, null, 8, ["image", "src", "svg", "alt"])
          ])
        ]),
        n("div", Mi, [
          n("h3", Ti, [
            n("span", Fi, [
              n("a", {
                href: e.href,
                class: "focus:outline-none"
              }, [
                l[0] || (l[0] = n("span", {
                  class: "absolute inset-0",
                  "aria-hidden": "true"
                }, null, -1)),
                we(" " + M(e.title), 1)
              ], 8, Ai)
            ])
          ]),
          n("p", Ii, [
            H(t.$slots, "default")
          ])
        ]),
        l[1] || (l[1] = n("div", { class: "flex-shrink-0 self-center" }, [
          n("svg", {
            class: "h-5 w-5 text-gray-400",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "fill-rule": "evenodd",
              d: "M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z",
              "clip-rule": "evenodd"
            })
          ])
        ], -1))
      ]);
    };
  }
}), Oi = { key: 0 }, Pi = { class: "md:p-4" }, la = /* @__PURE__ */ ie({
  __name: "EnsureAccess",
  props: {
    invalidAccess: {},
    alertClass: {}
  },
  emits: ["done"],
  setup(e) {
    const { isAuthenticated: t } = On(), { config: l } = Bt(), s = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const u = Qt(l.value.redirectSignIn, { redirect: r });
      l.value.navigate(u);
    }, o = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const u = Qt(l.value.redirectSignOut, { ReturnUrl: r });
      l.value.navigate(u);
    };
    return (r, u) => {
      const d = z("Alert"), f = z("SecondaryButton");
      return e.invalidAccess ? (a(), i("div", Oi, [
        he(d, {
          class: m(e.alertClass),
          innerHTML: e.invalidAccess
        }, null, 8, ["class", "innerHTML"]),
        n("div", Pi, [
          W(t) ? (a(), X(f, {
            key: 1,
            onClick: o
          }, {
            default: xe(() => [...u[1] || (u[1] = [
              we("Sign Out", -1)
            ])]),
            _: 1
          })) : (a(), X(f, {
            key: 0,
            onClick: s
          }, {
            default: xe(() => [...u[0] || (u[0] = [
              we("Sign In", -1)
            ])]),
            _: 1
          }))
        ])
      ])) : x("", !0);
    };
  }
}), ji = { class: "absolute top-0 right-0 bg-white dark:bg-black border dark:border-gray-800 rounded normal-case text-sm shadow w-80" }, Bi = { class: "p-4" }, Hi = { class: "flex w-full justify-center" }, Ri = { key: 0 }, Ei = ["id", "value"], Ni = ["for"], zi = { key: 1 }, Ui = { class: "mb-2" }, qi = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, _i = ["onClick"], Qi = { class: "flex" }, Ki = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, Pn = /* @__PURE__ */ ie({
  __name: "FilterColumn",
  props: {
    definitions: {},
    column: {},
    topLeft: {}
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const l = e, s = t, o = T(), r = T(""), u = T(""), d = T([]), f = c(() => l.column.meta.isEnum == !0), v = c(() => it(l.column.meta.type === "Nullable`1" ? l.column.meta.genericArgs[0] : l.column.meta.type)), y = c(() => l.column.meta.isEnum == !0 ? Cn($s(v.value.name)) : []), L = c(() => h(l.column.type)?.map((S) => ({ key: S.value, value: S.name })) || []), w = T({ filters: [] }), g = c(() => w.value.filters);
    kl(() => w.value = Object.assign({}, l.column.settings, {
      filters: Array.from(l.column.settings.filters)
    })), kl(() => {
      let S = l.column.settings.filters?.[0]?.value?.split(",") || [];
      if (S.length > 0 && v.value?.isEnumInt) {
        const V = S[0] && parseInt(S[0]) || 0;
        S = v.value.enumValues?.filter((ee) => (V & parseInt(ee)) > 0) || [];
      }
      d.value = S;
    });
    function h(S) {
      let V = l.definitions;
      return ws(S) || (V = V.filter((ee) => ee.types !== "string")), V;
    }
    function b(S, V) {
      return h(S).find((ee) => ee.value === V);
    }
    function A() {
      if (!r.value) return;
      let S = b(l.column.type, r.value)?.name;
      S && (w.value.filters.push({ key: r.value, name: S, value: u.value }), r.value = u.value = "");
    }
    function B(S) {
      w.value.filters.splice(S, 1);
    }
    function le(S) {
      return Ts(b(l.column.type, S.key), l.column.type, S);
    }
    function E() {
      s("done");
    }
    function I() {
      r.value = "%", o.value?.focus();
    }
    function K() {
      if (u.value && A(), f.value) {
        let S = Object.values(d.value).filter((V) => V);
        w.value.filters = S.length > 0 ? v.value?.isEnumInt ? [{ key: "%HasAny", name: "HasAny", value: S.map((V) => parseInt(V)).reduce((V, ee) => V + ee, 0).toString() }] : [{ key: "%In", name: "In", value: S.join(",") }] : [];
      }
      s("save", w.value), s("done");
    }
    function U(S) {
      w.value.sort = S === w.value.sort ? void 0 : S, At(K);
    }
    return (S, V) => {
      const ee = z("SelectInput"), re = z("TextInput"), O = z("PrimaryButton"), Z = z("SecondaryButton");
      return a(), i("div", {
        class: "fixed z-20 inset-0 overflow-y-auto",
        onClick: E,
        onVnodeMounted: I
      }, [
        n("div", {
          class: "absolute",
          style: un(`top:${e.topLeft.y}px;left:${e.topLeft.x}px`),
          onClick: V[5] || (V[5] = He(() => {
          }, ["stop"]))
        }, [
          n("div", ji, [
            n("div", Bi, [
              V[10] || (V[10] = n("h3", { class: "text-base font-medium mb-3 dark:text-gray-100" }, "Sort", -1)),
              n("div", Hi, [
                n("button", {
                  type: "button",
                  title: "Sort Ascending",
                  onClick: V[0] || (V[0] = (_) => U("ASC")),
                  class: m(`${w.value.sort === "ASC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} mr-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, [...V[6] || (V[6] = [
                  n("svg", {
                    class: "w-6 h-6",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 16 16"
                  }, [
                    n("g", { fill: "currentColor" }, [
                      n("path", {
                        "fill-rule": "evenodd",
                        d: "M10.082 5.629L9.664 7H8.598l1.789-5.332h1.234L13.402 7h-1.12l-.419-1.371h-1.781zm1.57-.785L11 2.687h-.047l-.652 2.157h1.351z"
                      }),
                      n("path", { d: "M12.96 14H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V14zm-8.46-.5a.5.5 0 0 1-1 0V3.707L2.354 4.854a.5.5 0 1 1-.708-.708l2-1.999l.007-.007a.498.498 0 0 1 .7.006l2 2a.5.5 0 1 1-.707.708L4.5 3.707V13.5z" })
                    ])
                  ], -1),
                  n("span", null, "ASC", -1)
                ])], 2),
                n("button", {
                  type: "button",
                  title: "Sort Descending",
                  onClick: V[1] || (V[1] = (_) => U("DESC")),
                  class: m(`${w.value.sort === "DESC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} ml-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, [...V[7] || (V[7] = [
                  Sl('<svg class="w-6 h-6" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16"><g fill="currentColor"><path d="M12.96 7H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V7z"></path><path fill-rule="evenodd" d="M10.082 12.629L9.664 14H8.598l1.789-5.332h1.234L13.402 14h-1.12l-.419-1.371h-1.781zm1.57-.785L11 9.688h-.047l-.652 2.156h1.351z"></path><path d="M4.5 2.5a.5.5 0 0 0-1 0v9.793l-1.146-1.147a.5.5 0 0 0-.708.708l2 1.999l.007.007a.497.497 0 0 0 .7-.006l2-2a.5.5 0 0 0-.707-.708L4.5 12.293V2.5z"></path></g></svg><span>DESC</span>', 2)
                ])], 2)
              ]),
              V[11] || (V[11] = n("h3", { class: "text-base font-medium mt-4 mb-2" }, " Filter ", -1)),
              f.value ? (a(), i("div", Ri, [
                (a(!0), i($e, null, Se(y.value, (_) => (a(), i("div", {
                  key: _.key,
                  class: "flex items-center"
                }, [
                  It(n("input", {
                    type: "checkbox",
                    id: _.key,
                    value: _.key,
                    "onUpdate:modelValue": V[2] || (V[2] = (ne) => d.value = ne),
                    class: "h-4 w-4 border-gray-300 rounded text-indigo-600 focus:ring-indigo-500"
                  }, null, 8, Ei), [
                    [dn, d.value]
                  ]),
                  n("label", {
                    for: _.key,
                    class: "ml-3"
                  }, M(_.value), 9, Ni)
                ]))), 128))
              ])) : (a(), i("div", zi, [
                (a(!0), i($e, null, Se(g.value, (_, ne) => (a(), i("div", Ui, [
                  n("span", qi, [
                    we(M(e.column.name) + " " + M(_.name) + " " + M(le(_)) + " ", 1),
                    n("button", {
                      type: "button",
                      onClick: (F) => B(ne),
                      class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                    }, [...V[8] || (V[8] = [
                      n("svg", {
                        class: "h-2 w-2",
                        stroke: "currentColor",
                        fill: "none",
                        viewBox: "0 0 8 8"
                      }, [
                        n("path", {
                          "stroke-linecap": "round",
                          "stroke-width": "1.5",
                          d: "M1 1l6 6m0-6L1 7"
                        })
                      ], -1)
                    ])], 8, _i)
                  ])
                ]))), 256)),
                n("div", Qi, [
                  he(ee, {
                    id: "filterRule",
                    class: "w-32 mr-1",
                    modelValue: r.value,
                    "onUpdate:modelValue": V[3] || (V[3] = (_) => r.value = _),
                    entries: L.value,
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue", "entries"]),
                  b(e.column.type, r.value)?.valueType !== "none" ? (a(), X(re, {
                    key: 0,
                    ref_key: "txtFilter",
                    ref: o,
                    id: "filterValue",
                    class: "w-32 mr-1",
                    type: "text",
                    modelValue: u.value,
                    "onUpdate:modelValue": V[4] || (V[4] = (_) => u.value = _),
                    onKeyup: as(A, ["enter"]),
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue"])) : x("", !0),
                  n("div", { class: "pt-1" }, [
                    n("button", {
                      type: "button",
                      onClick: A,
                      class: "inline-flex items-center p-1 border border-transparent rounded-full shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    }, [...V[9] || (V[9] = [
                      n("svg", {
                        class: "h-6 w-6",
                        xmlns: "http://www.w3.org/2000/svg",
                        viewBox: "0 0 20 20",
                        fill: "currentColor",
                        "aria-hidden": "true"
                      }, [
                        n("path", {
                          "fill-rule": "evenodd",
                          d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
                          "clip-rule": "evenodd"
                        })
                      ], -1)
                    ])])
                  ])
                ])
              ]))
            ]),
            n("div", Ki, [
              he(O, {
                onClick: K,
                color: "red",
                class: "ml-2"
              }, {
                default: xe(() => [...V[12] || (V[12] = [
                  we(" Save ", -1)
                ])]),
                _: 1
              }),
              he(Z, { onClick: E }, {
                default: xe(() => [...V[13] || (V[13] = [
                  we(" Cancel ", -1)
                ])]),
                _: 1
              })
            ])
          ])
        ], 4)
      ], 512);
    };
  }
}), Zi = { class: "px-4 sm:px-6 lg:px-8 text-sm" }, Wi = { class: "flex flex-wrap" }, Gi = { class: "group pr-4 sm:pr-6 lg:pr-8" }, Ji = { class: "flex justify-between w-full font-medium" }, Xi = { class: "w-6 flex justify-end" }, Yi = { class: "hidden group-hover:inline" }, er = ["onClick", "title"], tr = {
  key: 0,
  class: "pt-2"
}, lr = { class: "ml-2" }, nr = { key: 1 }, sr = { class: "pt-2" }, ar = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, or = ["onClick"], jn = /* @__PURE__ */ ie({
  __name: "FilterViews",
  props: {
    definitions: {},
    columns: {}
  },
  emits: ["done", "change"],
  setup(e, { emit: t }) {
    const l = e, s = t, o = c(() => l.columns.filter((w) => w.settings.filters.length > 0));
    function r(w) {
      return w?.[0]?.value?.split(",");
    }
    function u(w) {
      let g = l.definitions;
      return ws(w) || (g = g.filter((h) => h.types !== "string")), g;
    }
    function d(w, g) {
      return u(w).find((h) => h.value === g);
    }
    function f(w, g) {
      return Ts(d(w.type, g.value), w.type, g);
    }
    function v(w) {
      w.settings.filters = [], s("change", w);
    }
    function y(w, g) {
      w.settings.filters.splice(g, 1), s("change", w);
    }
    function L() {
      l.columns.forEach((w) => {
        w.settings.filters = [], s("change", w);
      }), s("done");
    }
    return (w, g) => (a(), i("div", Zi, [
      n("div", Wi, [
        (a(!0), i($e, null, Se(o.value, (h) => (a(), i("fieldset", Gi, [
          n("legend", Ji, [
            n("span", null, M(W(Pe)(h.name)), 1),
            n("span", Xi, [
              n("span", Yi, [
                n("button", {
                  onClick: (b) => v(h),
                  title: `Clear all ${W(Pe)(h.name)} filters`,
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-red-600 hover:bg-red-200 hover:text-red-500 focus:outline-none focus:bg-red-500 focus:text-white"
                }, [...g[0] || (g[0] = [
                  n("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    n("path", {
                      "stroke-linecap": "round",
                      "stroke-width": "1.5",
                      d: "M1 1l6 6m0-6L1 7"
                    })
                  ], -1)
                ])], 8, er)
              ])
            ])
          ]),
          h.meta.isEnum ? (a(), i("div", tr, [
            (a(!0), i($e, null, Se(r(h.settings.filters), (b) => (a(), i("div", {
              key: b,
              class: "flex items-center"
            }, [
              n("label", lr, M(b), 1)
            ]))), 128))
          ])) : (a(), i("div", nr, [
            (a(!0), i($e, null, Se(h.settings.filters, (b, A) => (a(), i("div", sr, [
              n("span", ar, [
                we(M(h.name) + " " + M(b.name) + " " + M(f(h, b)) + " ", 1),
                n("button", {
                  type: "button",
                  onClick: (B) => y(h, A),
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                }, [...g[1] || (g[1] = [
                  n("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    n("path", {
                      "stroke-linecap": "round",
                      "stroke-width": "1.5",
                      d: "M1 1l6 6m0-6L1 7"
                    })
                  ], -1)
                ])], 8, or)
              ])
            ]))), 256))
          ]))
        ]))), 256))
      ]),
      n("div", { class: "flex justify-center pt-4" }, [
        n("button", {
          type: "button",
          onClick: L,
          class: "inline-flex items-center px-2.5 py-1.5 border border-gray-300 shadow-sm text-sm font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        }, [...g[2] || (g[2] = [
          n("span", null, "Clear All", -1)
        ])])
      ])
    ]));
  }
}), ir = { class: "bg-white dark:bg-black px-4 pt-5 pb-4 sm:p-6 sm:pb-4" }, rr = { class: "" }, ur = { class: "mt-3 text-center sm:mt-0 sm:mx-4 sm:text-left" }, dr = { class: "mt-4" }, cr = ["for"], fr = ["id"], vr = ["value", "selected"], mr = { class: "mt-4 flex items-center py-4 border-b border-gray-200 dark:border-gray-800" }, gr = ["id", "checked"], pr = ["for"], yr = { class: "mt-4" }, hr = { class: "pb-2 px-4" }, br = { class: "" }, wr = ["id", "value"], xr = ["for"], kr = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, Bn = /* @__PURE__ */ ie({
  __name: "QueryPrefs",
  props: {
    id: { default: "QueryPrefs" },
    columns: {},
    prefs: {},
    maxLimit: {}
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const { autoQueryGridDefaults: l } = Bt(), s = e, o = t, r = T({});
    kl(() => r.value = Object.assign({
      take: l.value.take,
      selectedColumns: []
    }, s.prefs));
    const u = [10, 25, 50, 100, 250, 500, 1e3];
    function d() {
      o("done");
    }
    function f() {
      o("save", r.value);
    }
    return (v, y) => {
      const L = z("PrimaryButton"), w = z("SecondaryButton"), g = z("ModalDialog");
      return a(), X(g, {
        id: e.id,
        onDone: d,
        "size-class": "w-full sm:max-w-prose"
      }, {
        default: xe(() => [
          n("div", ir, [
            n("div", rr, [
              n("div", ur, [
                y[3] || (y[3] = n("h3", { class: "text-lg leading-6 font-medium text-gray-900 dark:text-gray-100" }, "Query Preferences", -1)),
                n("div", dr, [
                  n("label", {
                    for: `${e.id}-take`,
                    class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
                  }, "Results per page", 8, cr),
                  It(n("select", {
                    id: `${e.id}-take`,
                    "onUpdate:modelValue": y[0] || (y[0] = (h) => r.value.take = h),
                    class: "mt-1 block w-full pl-3 pr-10 py-2 text-base bg-white dark:bg-black border-gray-300 dark:border-gray-700 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                  }, [
                    (a(!0), i($e, null, Se(u.filter((h) => s.maxLimit == null || h <= s.maxLimit), (h) => (a(), i("option", {
                      value: h,
                      selected: h === r.value.take
                    }, M(h), 9, vr))), 256))
                  ], 8, fr), [
                    [va, r.value.take]
                  ])
                ]),
                n("div", mr, [
                  n("input", {
                    type: "radio",
                    id: `${e.id}-allColumns`,
                    onClick: y[1] || (y[1] = (h) => r.value.selectedColumns = []),
                    checked: r.value.selectedColumns.length === 0,
                    class: "focus:ring-indigo-500 h-4 w-4 bg-white dark:bg-black text-indigo-600 dark:text-indigo-400 border-gray-300 dark:border-gray-700"
                  }, null, 8, gr),
                  n("label", {
                    class: "ml-3 block text-gray-700 dark:text-gray-300",
                    for: `${e.id}-allColumns`
                  }, "View all columns", 8, pr)
                ]),
                n("div", yr, [
                  n("div", hr, [
                    n("div", br, [
                      (a(!0), i($e, null, Se(e.columns, (h) => (a(), i("div", {
                        key: h.name,
                        class: "flex items-center"
                      }, [
                        It(n("input", {
                          type: "checkbox",
                          id: h.name,
                          value: h.name,
                          "onUpdate:modelValue": y[2] || (y[2] = (b) => r.value.selectedColumns = b),
                          class: "h-4 w-4 bg-white dark:bg-black border-gray-300 dark:border-gray-700 rounded text-indigo-600 dark:text-indigo-400 focus:ring-indigo-500"
                        }, null, 8, wr), [
                          [dn, r.value.selectedColumns]
                        ]),
                        n("label", {
                          for: h.name,
                          class: "ml-3"
                        }, M(h.name), 9, xr)
                      ]))), 128))
                    ])
                  ])
                ])
              ])
            ])
          ]),
          n("div", kr, [
            he(L, {
              onClick: f,
              color: "red",
              class: "ml-2"
            }, {
              default: xe(() => [...y[4] || (y[4] = [
                we(" Save ", -1)
              ])]),
              _: 1
            }),
            he(w, { onClick: d }, {
              default: xe(() => [...y[5] || (y[5] = [
                we(" Cancel ", -1)
              ])]),
              _: 1
            })
          ])
        ]),
        _: 1
      }, 8, ["id"]);
    };
  }
}), Cr = { key: 0 }, $r = { key: 1 }, Lr = {
  key: 2,
  class: "pt-1"
}, Vr = { key: 0 }, Sr = { key: 1 }, Mr = { key: 2 }, Tr = { key: 4 }, Fr = { class: "pl-1 pt-1 flex flex-wrap" }, Ar = { class: "flex mt-1" }, Ir = ["title"], Dr = ["disabled"], Or = ["disabled"], Pr = ["disabled"], jr = ["disabled"], Br = {
  key: 0,
  class: "flex mt-1"
}, Hr = { class: "px-4 text-lg text-black dark:text-white" }, Rr = { key: 0 }, Er = { key: 1 }, Nr = { key: 2 }, zr = { class: "flex flex-wrap" }, Ur = {
  key: 0,
  class: "pl-2 mt-1"
}, qr = {
  key: 1,
  class: "pl-2 mt-1"
}, _r = {
  key: 2,
  class: "pl-2 mt-1"
}, Qr = {
  key: 0,
  class: "w-5 h-5 mr-1 text-green-600 dark:text-green-400",
  fill: "none",
  stroke: "currentColor",
  viewBox: "0 0 24 24",
  xmlns: "http://www.w3.org/2000/svg"
}, Kr = {
  key: 1,
  class: "w-5 h-5 mr-1",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, Zr = {
  key: 3,
  class: "pl-2 mt-1"
}, Wr = {
  key: 4,
  class: "pl-2 mt-1"
}, Gr = { class: "mr-1" }, Jr = {
  key: 0,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Xr = {
  key: 1,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Yr = {
  key: 5,
  class: "pl-2 mt-1"
}, eu = ["title"], tu = { class: "whitespace-nowrap" }, lu = { key: 8 }, nu = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, su = { class: "mr-1 select-none" }, au = {
  key: 1,
  class: "flex justify-between items-center"
}, ou = { class: "mr-1 select-none" }, yl = 25, iu = /* @__PURE__ */ ie({
  __name: "AutoQueryGrid",
  props: {
    filterDefinitions: {},
    id: { default: "AutoQueryGrid" },
    apis: {},
    type: {},
    prefs: {},
    deny: {},
    hide: {},
    selectedColumns: {},
    toolbarButtonClass: {},
    tableStyle: {},
    gridClass: {},
    grid2Class: {},
    grid3Class: {},
    grid4Class: {},
    tableClass: {},
    theadClass: {},
    tbodyClass: {},
    theadRowClass: {},
    theadCellClass: {},
    headerTitle: {},
    headerTitles: {},
    visibleFrom: {},
    rowClass: {},
    rowStyle: {},
    modelTitle: {},
    newButtonLabel: {},
    apiPrefs: {},
    canFilter: {},
    disableKeyBindings: {},
    configureField: {},
    skip: { default: 0 },
    create: { type: Boolean },
    edit: {},
    filters: {}
  },
  emits: ["headerSelected", "rowSelected", "nav"],
  setup(e, { expose: t, emit: l }) {
    const { config: s, autoQueryGridDefaults: o } = Bt(), r = o, u = s.value.storage, d = e, f = l, v = ze("client"), y = "filtering,queryString,queryFilters".split(","), L = "copyApiUrl,downloadCsv,filtersView,newItem,pagingInfo,pagingNav,preferences,refresh,resetPreferences,toolbar,forms".split(","), w = c(() => d.deny ? Ft(y, d.deny) : Ft(y, r.value.deny)), g = c(() => d.hide ? Ft(L, d.hide) : Ft(L, r.value.hide));
    function h(D) {
      return w.value[D];
    }
    function b(D) {
      return g.value[D];
    }
    const A = c(() => d.tableStyle ?? r.value.tableStyle), B = c(() => d.gridClass ?? ge.getGridClass(A.value)), le = c(() => d.grid2Class ?? ge.getGrid2Class(A.value)), E = c(() => d.grid3Class ?? ge.getGrid3Class(A.value)), I = c(() => d.grid4Class ?? ge.getGrid4Class(A.value)), K = c(() => d.tableClass ?? ge.getTableClass(A.value)), U = c(() => d.theadClass ?? ge.getTheadClass(A.value)), S = c(() => d.theadRowClass ?? ge.getTheadRowClass(A.value)), V = c(() => d.theadCellClass ?? ge.getTheadCellClass(A.value)), ee = c(() => d.toolbarButtonClass ?? ge.toolbarButtonClass);
    function re(D, $) {
      if (d.rowClass) return d.rowClass(D, $);
      const me = !!ye.value.AnyUpdate, Ae = (ce.value?.name ? be(D, ce.value.name) : null) == te.value;
      return ge.getTableRowClass(d.tableStyle, $, Ae, me);
    }
    const O = Ml(), Z = c(() => Ul(ye.value.AnyQuery.viewModel?.name || ye.value.AnyQuery.dataModel.name)), _ = c(() => {
      const D = Object.keys(O).map(($) => $.toLowerCase());
      return Xe(Z.value).filter(($) => D.includes($.name.toLowerCase()) || D.includes($.name.toLowerCase() + "-header")).map(($) => $.name);
    });
    function ne() {
      let D = Tt(d.selectedColumns);
      return D.length > 0 ? D : _.value.length > 0 ? _.value : [];
    }
    const F = c(() => {
      let $ = ne().map((ae) => ae.toLowerCase());
      const me = Xe(Z.value);
      return $.length > 0 ? $.map((ae) => me.find((Ae) => Ae.name.toLowerCase() === ae)).filter((ae) => ae != null) : me;
    }), J = c(() => {
      let D = F.value.map((me) => me.name), $ = Tt(fe.value.selectedColumns).map((me) => me.toLowerCase());
      return $.length > 0 ? D.filter((me) => $.includes(me.toLowerCase())) : D;
    }), C = T([]), P = T(new We()), Q = T(new We()), k = T(), p = T(!1), te = T(), Y = T(), oe = T(!1), N = T(), j = T(d.skip), ue = T(!1), fe = T({ take: yl }), se = T(!1), ve = c(() => C.value.some((D) => D.settings.filters.length > 0 || !!D.settings.sort) || fe.value.selectedColumns), pe = c(() => C.value.map((D) => D.settings.filters.length).reduce((D, $) => D + $, 0)), de = c(() => Xe(Ul(Et.value || ye.value.AnyQuery?.dataModel.name))), ce = c(() => Wt(Ul(Et.value || ye.value.AnyQuery?.dataModel.name))), Le = c(() => fe.value.take ?? yl), Ie = c(() => (P.value.response ? be(P.value.response, "results") : null) ?? []), Ce = c(() => (P.value.response?.total || Ie.value.length) ?? 0), Me = c(() => j.value > 0), Te = c(() => j.value > 0), qe = c(() => Ie.value.length >= Le.value), nt = c(() => Ie.value.length >= Le.value), et = T(), rt = T(), ut = {
      NoQuery: "No Query API was found"
    };
    t({
      update: Ve,
      search: Hn,
      createRequestArgs: Nl,
      reset: Kn,
      createDone: Yt,
      createSave: Ql,
      editDone: $t,
      editSave: Nt,
      forceUpdate: Ct,
      setEdit: ht,
      edit: Y,
      createForm: et,
      editForm: rt,
      apiPrefs: fe,
      results: Ie,
      skip: j,
      take: Le,
      total: Ce
    }), q.interceptors.has("AutoQueryGrid.new") && q.interceptors.invoke("AutoQueryGrid.new", { props: d });
    function vt(D) {
      if (D) {
        if (d.canFilter)
          return d.canFilter(D);
        const $ = de.value.find((me) => me.name.toLowerCase() == D.toLowerCase());
        if ($)
          return !xs($);
      }
      return !1;
    }
    function Qe(D) {
      f("nav", D), h("queryString") && Tn(D);
    }
    async function Re(D) {
      j.value += D, j.value < 0 && (j.value = 0);
      const $ = Math.floor(Ce.value / Le.value) * Le.value;
      j.value > $ && (j.value = $), Qe({ skip: j.value || void 0 }), await Ve();
    }
    async function st(D, $) {
      if (Y.value = null, te.value = $, !D || !$) return;
      let me = sl(ye.value.AnyQuery, { [D]: $ });
      const ae = await v.api(me);
      if (ae.succeeded) {
        let Ae = be(ae.response, "results")?.[0];
        Ae || console.warn(`API ${ye.value.AnyQuery?.request.name}(${D}:${$}) returned no results`), Y.value = Ae;
      }
    }
    async function yt(D, $) {
      f("rowSelected", D, $);
      const me = ce.value?.name, ae = me ? be(D, me) : null;
      !me || !ae || (Qe({ edit: ae }), st(me, ae));
    }
    function G(D, $) {
      if (!h("filtering")) return;
      let me = $.target;
      if (vt(D) && me?.tagName !== "TD") {
        let ae = me?.closest("TABLE")?.getBoundingClientRect(), Ae = C.value.find((je) => je.name.toLowerCase() == D.toLowerCase());
        if (Ae && ae) {
          let je = 318, Ge = ae.x + je + 10;
          N.value = {
            column: Ae,
            topLeft: {
              x: Math.max(Math.floor($.clientX + je / 2), Ge),
              y: ae.y + 45
            }
          };
        }
      }
      f("headerSelected", D, $);
    }
    function R() {
      N.value = null;
    }
    async function Fe(D) {
      let $ = N.value?.column;
      $ && ($.settings = D, u.setItem(fl($.name), JSON.stringify($.settings)), await Ve()), N.value = null;
    }
    async function De(D) {
      u.setItem(fl(D.name), JSON.stringify(D.settings)), await Ve();
    }
    async function _e(D) {
      oe.value = !1, fe.value = D, u.setItem(zl(), JSON.stringify(D)), await Ve();
    }
    function tt(D) {
      et.value && (Object.assign(et.value?.model, D), Ct());
    }
    function ht(D) {
      Object.assign(Y.value, D), Ct();
    }
    function Ct() {
      et.value?.forceUpdate(), rt.value?.forceUpdate(), Oe()?.proxy?.$forceUpdate();
    }
    async function Ve() {
      await Hn(Nl());
    }
    async function Rt() {
      await Ve();
    }
    const El = /iPad|iPhone|iPod/.test(navigator.userAgent);
    async function Hn(D) {
      const $ = ye.value.AnyQuery;
      if (!$) {
        console.error(ut.NoQuery);
        return;
      }
      let me = sl($, D), ae = await v.api(me);
      ds((Ge) => {
        P.value.response = P.value.error = void 0, se.value = Ge, El ? At(() => P.value = ae) : P.value = ae;
      })();
      let je = be(ae.response, "results") || [];
      !ae.succeeded || je.label == 0;
    }
    function Nl() {
      let D = {
        include: "total",
        take: Le.value
      }, $ = Tt(fe.value.selectedColumns || d.selectedColumns);
      if ($.length > 0) {
        let ae = ce.value;
        ae && !$.includes(ae.name) && ($ = [ae.name, ...$]);
        const Ae = de.value, je = [];
        $.forEach((Ge) => {
          const zt = Ae.find((Lt) => Lt.name.toLowerCase() == Ge.toLowerCase());
          zt?.ref?.selfId && je.push(zt.ref.selfId), be(O, Ge) && je.push(...Ae.filter((Lt) => Lt.ref?.selfId?.toLowerCase() == Ge.toLowerCase()).map((Lt) => Lt.name));
        }), je.forEach((Ge) => {
          $.includes(Ge) || $.push(Ge);
        }), D.fields = Js($).join(",");
      }
      let me = [];
      if (C.value.forEach((ae) => {
        ae.settings.sort && me.push((ae.settings.sort === "DESC" ? "-" : "") + ae.name), ae.settings.filters.forEach((Ae) => {
          let je = Ae.key.replace("%", ae.name);
          D[je] = Ae.value;
        });
      }), d.filters && Object.keys(d.filters).forEach((ae) => {
        D[ae] = d.filters[ae];
      }), h("queryString") && h("queryFilters")) {
        const ae = location.search ? location.search : location.hash.includes("?") ? "?" + hl(location.hash, "?") : "";
        let Ae = Jl(ae);
        if (Object.keys(Ae).forEach((je) => {
          F.value.find((zt) => zt.name.toLowerCase() === je.toLowerCase()) && (D[je] = Ae[je]);
        }), typeof Ae.skip < "u") {
          const je = parseInt(Ae.skip);
          isNaN(je) || (j.value = D.skip = je);
        }
      }
      return typeof D.skip > "u" && j.value > 0 && (D.skip = j.value), me.length > 0 && (D.orderBy = me.join(",")), D;
    }
    function na() {
      const D = Rn("csv");
      on(D), typeof window < "u" && window.open(D);
    }
    function sa() {
      const D = Rn("json");
      on(D), ue.value = !0, setTimeout(() => ue.value = !1, 3e3);
    }
    function Rn(D = "json") {
      const $ = Nl(), me = `/api/${ye.value.AnyQuery?.request.name}`, ae = Fa(v.baseUrl, Qt(me, { ...$, jsconfig: "edv" }));
      return ae.indexOf("?") >= 0 ? Tl(ae, "?") + "." + D + "?" + hl(ae, "?") : ae + ".json";
    }
    async function aa() {
      C.value.forEach((D) => {
        D.settings = { filters: [] }, u.removeItem(fl(D.name));
      }), fe.value = { take: yl }, u.removeItem(zl()), await Ve();
    }
    function oa() {
      p.value = !0, Qe({ create: null });
    }
    const Et = c(() => Ht(d.type)), Jt = c(() => Et.value || ye.value.AnyQuery?.dataModel.name), Xt = c(() => d.modelTitle || Jt.value), ia = c(() => d.newButtonLabel || `New ${Xt.value}`), zl = () => `${d.id}/ApiPrefs/${Et.value || ye.value.AnyQuery?.dataModel.name}`, fl = (D) => `Column/${d.id}:${Et.value || ye.value.AnyQuery?.dataModel.name}.${D}`, { metadataApi: En, typeOf: Ul, apiOf: Nn, filterDefinitions: ra } = lt(), { invalidAccessMessage: ql } = On(), zn = c(() => d.filterDefinitions || ra.value), ye = c(() => {
      let D = Tt(d.apis);
      return D.length > 0 ? Ot.from(D.map(($) => Nn($)).filter(($) => $ != null).map(($) => $)) : Ot.forType(Et.value, En.value);
    }), vl = (D) => `<span class="text-yellow-700">${D}</span>`, Un = c(() => {
      if (!En.value)
        return vl(`AppMetadata not loaded, see <a class="${Vl.blue}" href="https://docs.servicestack.net/vue/use-metadata" target="_blank">useMetadata()</a>`);
      let $ = Tt(d.apis).map((ae) => Nn(ae) == null ? ae : null).filter((ae) => ae != null);
      if ($.length > 0)
        return vl(`Unknown API${$.length > 1 ? "s" : ""}: ${$.join(", ")}`);
      let me = ye.value;
      return me.empty ? vl("Mising DataModel in property 'type' or AutoQuery APIs to use in property 'apis'") : me.AnyQuery ? null : vl(ut.NoQuery);
    }), qn = c(() => ye.value.AnyQuery && ql(ye.value.AnyQuery)), _n = c(() => ye.value.Create && ql(ye.value.Create)), Qn = c(() => ye.value.AnyUpdate && ql(ye.value.AnyUpdate)), ua = c(() => nl(ye.value.Create));
    c(() => nl(ye.value.AnyUpdate));
    const _l = c(() => nl(ye.value.Delete));
    function $t() {
      Y.value = null, te.value = null, Qe({ edit: void 0 });
    }
    function Yt() {
      p.value = !1, Qe({ create: void 0 });
    }
    async function Nt() {
      await Ve(), $t();
    }
    async function Ql() {
      await Ve(), Yt();
    }
    function Kn() {
      P.value = new We(), Q.value = new We(), p.value = !1, te.value = null, Y.value = null, oe.value = !1, N.value = null, j.value = d.skip, ue.value = !1, fe.value = { take: yl }, se.value = !1;
      const D = d.prefs || Ll(u.getItem(zl()));
      D && (fe.value = D), C.value = F.value.map((me) => ({
        name: me.name,
        type: me.type,
        meta: me,
        settings: Object.assign(
          {
            filters: []
          },
          Ll(u.getItem(fl(me.name)))
        )
      })), isNaN(d.skip) || (j.value = d.skip);
      let $ = ce.value?.name;
      if (h("queryString")) {
        const me = location.search ? location.search : location.hash.includes("?") ? "?" + hl(location.hash, "?") : "";
        let ae = Jl(me);
        typeof ae.create < "u" ? p.value = typeof ae.create < "u" : $ && (typeof ae.edit == "string" || typeof ae.edit == "number") && st($, ae.edit);
      }
      d.create === !0 && (p.value = !0), $ && d.edit != null && st($, d.edit);
    }
    return Ye(async () => {
      Kn(), await Ve();
    }), (D, $) => {
      const me = z("Alert"), ae = z("EnsureAccessDialog"), Ae = z("AutoCreateForm"), je = z("AutoEditForm"), Ge = z("AutoViewForm"), zt = z("ErrorSummary"), Zn = z("Loading"), Lt = z("SettingsIcons"), da = z("DataGrid");
      return Un.value ? (a(), i("div", Cr, [
        he(me, { innerHTML: Un.value }, null, 8, ["innerHTML"])
      ])) : qn.value ? (a(), i("div", $r, [
        he(la, { "invalid-access": qn.value }, null, 8, ["invalid-access"])
      ])) : (a(), i("div", Lr, [
        b("forms") && p.value && ye.value.Create ? (a(), i("div", Vr, [
          _n.value ? (a(), X(ae, {
            key: 0,
            title: `Create ${Xt.value}`,
            "invalid-access": _n.value,
            "alert-class": "text-yellow-700",
            onDone: Yt
          }, null, 8, ["title", "invalid-access"])) : W(O).createform ? H(D.$slots, "createform", {
            key: 1,
            type: ye.value.Create.request.name,
            configure: e.configureField,
            done: Yt,
            save: Ql
          }) : (a(), X(Ae, {
            key: 2,
            ref_key: "createForm",
            ref: et,
            type: ye.value.Create.request.name,
            configure: e.configureField,
            onDone: Yt,
            onSave: Ql
          }, {
            header: xe(() => [
              H(D.$slots, "formheader", {
                form: "create",
                formInstance: et.value,
                apis: ye.value,
                type: Jt.value,
                updateModel: tt
              })
            ]),
            footer: xe(() => [
              H(D.$slots, "formfooter", {
                form: "create",
                formInstance: et.value,
                apis: ye.value,
                type: Jt.value,
                updateModel: tt
              })
            ]),
            _: 3
          }, 8, ["type", "configure"]))
        ])) : b("forms") && Y.value && ye.value.AnyUpdate ? (a(), i("div", Sr, [
          Qn.value ? (a(), X(ae, {
            key: 0,
            title: `Update ${Xt.value}`,
            "invalid-access": Qn.value,
            "alert-class": "text-yellow-700",
            onDone: $t
          }, null, 8, ["title", "invalid-access"])) : W(O).editform ? H(D.$slots, "editform", {
            key: 1,
            model: Y.value,
            type: ye.value.AnyUpdate.request.name,
            deleteType: _l.value ? ye.value.Delete.request.name : null,
            configure: e.configureField,
            done: $t,
            save: Nt
          }) : (a(), X(je, {
            key: 2,
            ref_key: "editForm",
            ref: rt,
            modelValue: Y.value,
            "onUpdate:modelValue": $[0] || ($[0] = (Ue) => Y.value = Ue),
            type: ye.value.AnyUpdate.request.name,
            deleteType: _l.value ? ye.value.Delete.request.name : null,
            configure: e.configureField,
            onDone: $t,
            onSave: Nt,
            onDelete: Nt
          }, {
            header: xe(() => [
              H(D.$slots, "formheader", {
                form: "edit",
                formInstance: rt.value,
                apis: ye.value,
                type: Jt.value,
                model: Y.value,
                id: te.value,
                updateModel: ht
              })
            ]),
            footer: xe(() => [
              H(D.$slots, "formfooter", {
                form: "edit",
                formInstance: rt.value,
                apis: ye.value,
                type: Jt.value,
                model: Y.value,
                id: te.value,
                updateModel: ht
              })
            ]),
            _: 3
          }, 8, ["modelValue", "type", "deleteType", "configure"]))
        ])) : b("forms") && Y.value ? (a(), i("div", Mr, [
          W(O).viewform ? H(D.$slots, "viewform", {
            key: 0,
            model: Y.value,
            apis: ye.value,
            done: $t
          }) : (a(), X(Ge, {
            key: 1,
            model: Y.value,
            apis: ye.value,
            deleteType: _l.value ? ye.value.Delete.request.name : null,
            done: $t,
            onSave: Nt,
            onDelete: Nt
          }, null, 8, ["model", "apis", "deleteType"]))
        ])) : x("", !0),
        W(O).toolbar ? H(D.$slots, "toolbar", { key: 3 }) : b("toolbar") ? (a(), i("div", Tr, [
          oe.value ? (a(), X(Bn, {
            key: 0,
            columns: F.value,
            prefs: fe.value,
            onDone: $[1] || ($[1] = (Ue) => oe.value = !1),
            onSave: _e
          }, null, 8, ["columns", "prefs"])) : x("", !0),
          n("div", Fr, [
            n("div", Ar, [
              b("preferences") ? (a(), i("button", {
                key: 0,
                type: "button",
                class: "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                title: `${Xt.value} Preferences`,
                onClick: $[2] || ($[2] = (Ue) => oe.value = !oe.value)
              }, [...$[9] || ($[9] = [
                n("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  n("g", {
                    "stroke-width": "1.5",
                    fill: "none"
                  }, [
                    n("path", {
                      d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
                      stroke: "currentColor"
                    })
                  ])
                ], -1)
              ])], 8, Ir)) : x("", !0),
              b("pagingNav") ? (a(), i("button", {
                key: 1,
                type: "button",
                class: m(["pl-2", Me.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "First page",
                disabled: !Me.value,
                onClick: $[3] || ($[3] = (Ue) => Re(-Ce.value))
              }, [...$[10] || ($[10] = [
                n("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  n("path", {
                    d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Dr)) : x("", !0),
              b("pagingNav") ? (a(), i("button", {
                key: 2,
                type: "button",
                class: m(["pl-2", Te.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Previous page",
                disabled: !Te.value,
                onClick: $[4] || ($[4] = (Ue) => Re(-Le.value))
              }, [...$[11] || ($[11] = [
                n("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  n("path", {
                    d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Or)) : x("", !0),
              b("pagingNav") ? (a(), i("button", {
                key: 3,
                type: "button",
                class: m(["pl-2", qe.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Next page",
                disabled: !qe.value,
                onClick: $[5] || ($[5] = (Ue) => Re(Le.value))
              }, [...$[12] || ($[12] = [
                n("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  n("path", {
                    d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Pr)) : x("", !0),
              b("pagingNav") ? (a(), i("button", {
                key: 4,
                type: "button",
                class: m(["pl-2", nt.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Last page",
                disabled: !nt.value,
                onClick: $[6] || ($[6] = (Ue) => Re(Ce.value))
              }, [...$[13] || ($[13] = [
                n("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  n("path", {
                    d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, jr)) : x("", !0)
            ]),
            b("pagingInfo") ? (a(), i("div", Br, [
              n("div", Hr, [
                se.value ? (a(), i("span", Rr, "Querying...")) : x("", !0),
                Ie.value.length ? (a(), i("span", Er, [
                  $[14] || ($[14] = n("span", { class: "hidden xl:inline" }, " Showing Results ", -1)),
                  we(" " + M(j.value + 1) + " - " + M(Math.min(j.value + Ie.value.length, Ce.value)) + " ", 1),
                  n("span", null, " of " + M(Ce.value), 1)
                ])) : P.value.completed ? (a(), i("span", Nr, "No Results")) : x("", !0)
              ])
            ])) : x("", !0),
            n("div", zr, [
              b("refresh") ? (a(), i("div", Ur, [
                n("button", {
                  type: "button",
                  onClick: Rt,
                  title: "Refresh",
                  class: m(ee.value)
                }, [...$[15] || ($[15] = [
                  n("svg", {
                    class: "w-5 h-5",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 24 24"
                  }, [
                    n("path", {
                      fill: "none",
                      stroke: "currentColor",
                      "stroke-linecap": "round",
                      "stroke-linejoin": "round",
                      "stroke-width": "2",
                      d: "M20 20v-5h-5M4 4v5h5m10.938 2A8.001 8.001 0 0 0 5.07 8m-1.008 5a8.001 8.001  0 0 0 14.868 3"
                    })
                  ], -1)
                ])], 2)
              ])) : x("", !0),
              b("downloadCsv") ? (a(), i("div", qr, [
                n("button", {
                  type: "button",
                  onClick: na,
                  title: "Download CSV",
                  class: m(ee.value)
                }, [...$[16] || ($[16] = [
                  Sl('<svg class="w-5 h-5 mr-1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32"><path d="M28.781 4.405h-10.13V2.018L2 4.588v22.527l16.651 2.868v-3.538h10.13A1.162 1.162 0 0 0 30 25.349V5.5a1.162 1.162 0 0 0-1.219-1.095zm.16 21.126H18.617l-.017-1.889h2.487v-2.2h-2.506l-.012-1.3h2.518v-2.2H18.55l-.012-1.3h2.549v-2.2H18.53v-1.3h2.557v-2.2H18.53v-1.3h2.557v-2.2H18.53v-2h10.411z" fill="#20744a" fill-rule="evenodd"></path><path fill="#20744a" d="M22.487 7.439h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 10.94h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 14.441h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 17.942h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 21.443h4.323v2.2h-4.323z"></path><path fill="#fff" fill-rule="evenodd" d="M6.347 10.673l2.146-.123l1.349 3.709l1.594-3.862l2.146-.123l-2.606 5.266l2.606 5.279l-2.269-.153l-1.532-4.024l-1.533 3.871l-2.085-.184l2.422-4.663l-2.238-4.993z"></path></svg><span class="text-green-900 dark:text-green-100">Excel</span>', 2)
                ])], 2)
              ])) : x("", !0),
              b("copyApiUrl") ? (a(), i("div", _r, [
                n("button", {
                  type: "button",
                  onClick: sa,
                  title: "Copy API URL",
                  class: m(ee.value)
                }, [
                  ue.value ? (a(), i("svg", Qr, [...$[17] || ($[17] = [
                    n("path", {
                      "stroke-linecap": "round",
                      "stroke-linejoin": "round",
                      "stroke-width": "2",
                      d: "M5 13l4 4L19 7"
                    }, null, -1)
                  ])])) : (a(), i("svg", Kr, [...$[18] || ($[18] = [
                    n("g", { fill: "none" }, [
                      n("path", {
                        d: "M8 4v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7.242a2 2 0 0 0-.602-1.43L16.083 2.57A2 2 0 0 0 14.685 2H10a2 2 0 0 0-2 2z",
                        stroke: "currentColor",
                        "stroke-width": "2",
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round"
                      }),
                      n("path", {
                        d: "M16 18v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h2",
                        stroke: "currentColor",
                        "stroke-width": "2",
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round"
                      })
                    ], -1)
                  ])])),
                  $[19] || ($[19] = n("span", { class: "whitespace-nowrap" }, "Copy URL", -1))
                ], 2)
              ])) : x("", !0),
              ve.value && b("resetPreferences") ? (a(), i("div", Zr, [
                n("button", {
                  type: "button",
                  onClick: aa,
                  title: "Reset Preferences & Filters",
                  class: m(ee.value)
                }, [...$[20] || ($[20] = [
                  n("svg", {
                    class: "w-5 h-5",
                    xmlns: "http://www.w3.org/2000/svg",
                    "aria-hidden": "true",
                    viewBox: "0 0 24 24"
                  }, [
                    n("path", {
                      fill: "currentColor",
                      d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
                    })
                  ], -1)
                ])], 2)
              ])) : x("", !0),
              b("filtersView") && pe.value > 0 ? (a(), i("div", Wr, [
                n("button", {
                  type: "button",
                  onClick: $[7] || ($[7] = (Ue) => k.value = k.value == "filters" ? null : "filters"),
                  class: m(ee.value),
                  "aria-expanded": "false"
                }, [
                  $[23] || ($[23] = n("svg", {
                    class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
                    "aria-hidden": "true",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 20 20",
                    fill: "currentColor"
                  }, [
                    n("path", {
                      "fill-rule": "evenodd",
                      d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
                      "clip-rule": "evenodd"
                    })
                  ], -1)),
                  n("span", Gr, M(pe.value) + " " + M(pe.value == 1 ? "Filter" : "Filters"), 1),
                  k.value != "filters" ? (a(), i("svg", Jr, [...$[21] || ($[21] = [
                    n("path", {
                      "fill-rule": "evenodd",
                      d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
                      "clip-rule": "evenodd"
                    }, null, -1)
                  ])])) : (a(), i("svg", Xr, [...$[22] || ($[22] = [
                    n("path", {
                      "fill-rule": "evenodd",
                      d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
                      "clip-rule": "evenodd"
                    }, null, -1)
                  ])]))
                ], 2)
              ])) : x("", !0),
              b("newItem") && ye.value.Create && ua.value ? (a(), i("div", Yr, [
                n("button", {
                  type: "button",
                  onClick: oa,
                  title: Xt.value,
                  class: m(ee.value)
                }, [
                  $[24] || ($[24] = n("svg", {
                    class: "w-5 h-5 mr-1 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 24 24"
                  }, [
                    n("path", {
                      d: "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z",
                      fill: "currentColor"
                    })
                  ], -1)),
                  n("span", tu, M(ia.value), 1)
                ], 10, eu)
              ])) : x("", !0),
              W(O).toolbarbuttons ? H(D.$slots, "toolbarbuttons", {
                key: 6,
                toolbarButtonClass: ee.value
              }) : x("", !0)
            ])
          ])
        ])) : x("", !0),
        k.value == "filters" ? (a(), X(jn, {
          key: 5,
          class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
          definitions: zn.value,
          columns: C.value,
          onDone: $[8] || ($[8] = (Ue) => k.value = null),
          onChange: De
        }, null, 8, ["definitions", "columns"])) : x("", !0),
        Q.value.error ?? P.value.error ? (a(), X(zt, {
          key: 6,
          status: Q.value.error ?? P.value.error
        }, null, 8, ["status"])) : se.value ? (a(), X(Zn, {
          key: 7,
          class: "p-2"
        })) : x("", !0),
        N.value ? (a(), i("div", lu, [
          he(Pn, {
            definitions: zn.value,
            column: N.value.column,
            "top-left": N.value.topLeft,
            onDone: R,
            onSave: Fe
          }, null, 8, ["definitions", "column", "top-left"])
        ])) : x("", !0),
        Ie.value.length ? (a(), X(da, {
          key: 9,
          id: e.id,
          items: Ie.value,
          type: e.type,
          "selected-columns": J.value,
          class: "mt-1",
          onFiltersChanged: Ve,
          tableStyle: A.value,
          gridClass: B.value,
          grid2Class: le.value,
          grid3Class: E.value,
          grid4Class: I.value,
          tableClass: K.value,
          theadClass: U.value,
          theadRowClass: S.value,
          theadCellClass: V.value,
          tbodyClass: e.tbodyClass,
          rowClass: re,
          onRowSelected: yt,
          rowStyle: e.rowStyle,
          headerTitle: e.headerTitle,
          headerTitles: e.headerTitles,
          visibleFrom: e.visibleFrom,
          onHeaderSelected: G
        }, cn({
          header: xe(({ column: Ue, label: ml }) => [
            h("filtering") && vt(Ue) ? (a(), i("div", nu, [
              n("span", su, M(ml), 1),
              he(Lt, {
                column: C.value.find((ca) => ca.name.toLowerCase() === Ue.toLowerCase()),
                "is-open": N.value?.column.name === Ue
              }, null, 8, ["column", "is-open"])
            ])) : (a(), i("div", au, [
              n("span", ou, M(ml), 1)
            ]))
          ]),
          _: 2
        }, [
          Se(Object.keys(W(O)), (Ue) => ({
            name: Ue,
            fn: xe((ml) => [
              H(D.$slots, Ue, Cl(os(ml)))
            ])
          }))
        ]), 1032, ["id", "items", "type", "selected-columns", "tableStyle", "gridClass", "grid2Class", "grid3Class", "grid4Class", "tableClass", "theadClass", "theadRowClass", "theadCellClass", "tbodyClass", "rowStyle", "headerTitle", "headerTitles", "visibleFrom"])) : x("", !0)
      ]));
    };
  }
}), ru = { class: "flex" }, uu = {
  key: 0,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, du = {
  key: 2,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20"
}, cu = {
  key: 3,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20"
}, fu = /* @__PURE__ */ ie({
  __name: "SettingsIcons",
  props: {
    column: {},
    isOpen: { type: Boolean }
  },
  setup(e) {
    return (t, l) => (a(), i("div", ru, [
      e.column?.settings?.filters?.length ? (a(), i("svg", uu, [...l[0] || (l[0] = [
        n("g", { fill: "none" }, [
          n("path", {
            d: "M3 4a1 1 0 0 1 1-1h16a1 1 0 0 1 1 1v2.586a1 1 0 0 1-.293.707l-6.414 6.414a1 1 0 0 0-.293.707V17l-4 4v-6.586a1 1 0 0 0-.293-.707L3.293 7.293A1 1 0 0 1 3 6.586V4z",
            stroke: "currentColor",
            "stroke-width": "2",
            "stroke-linecap": "round",
            "stroke-linejoin": "round"
          })
        ], -1)
      ])])) : (a(), i("svg", {
        key: 1,
        class: m(["w-4 h-4 transition-transform", e.isOpen ? "rotate-180" : ""]),
        xmlns: "http://www.w3.org/2000/svg",
        viewBox: "0 0 1024 1024"
      }, [...l[1] || (l[1] = [
        n("path", {
          d: "M505.5 658.7c3.2 4.4 9.7 4.4 12.9 0l178-246c3.8-5.3 0-12.7-6.5-12.7H643c-10.2 0-19.9 4.9-25.9 13.2L512 558.6L406.8 413.2c-6-8.3-15.6-13.2-25.9-13.2H334c-6.5 0-10.3 7.4-6.5 12.7l178 246z",
          fill: "currentColor"
        }, null, -1),
        n("path", {
          d: "M880 112H144c-17.7 0-32 14.3-32 32v736c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V144c0-17.7-14.3-32-32-32zm-40 728H184V184h656v656z",
          fill: "currentColor"
        }, null, -1)
      ])], 2)),
      e.column?.settings?.sort === "ASC" ? (a(), i("svg", du, [...l[2] || (l[2] = [
        n("g", { fill: "none" }, [
          n("path", {
            d: "M8.998 4.71L6.354 7.354a.5.5 0 1 1-.708-.707L9.115 3.18A.499.499 0 0 1 9.498 3H9.5a.5.5 0 0 1 .354.147l.01.01l3.49 3.49a.5.5 0 1 1-.707.707l-2.65-2.649V16.5a.5.5 0 0 1-1 0V4.71z",
            fill: "currentColor"
          })
        ], -1)
      ])])) : e.column?.settings?.sort === "DESC" ? (a(), i("svg", cu, [...l[3] || (l[3] = [
        n("g", { fill: "none" }, [
          n("path", {
            d: "M10.002 15.29l2.645-2.644a.5.5 0 0 1 .707.707L9.886 16.82a.5.5 0 0 1-.384.179h-.001a.5.5 0 0 1-.354-.147l-.01-.01l-3.49-3.49a.5.5 0 1 1 .707-.707l2.648 2.649V3.5a.5.5 0 0 1 1 0v11.79z",
            fill: "currentColor"
          })
        ], -1)
      ])])) : x("", !0)
    ]));
  }
}), vu = /* @__PURE__ */ ie({
  __name: "EnsureAccessDialog",
  props: {
    title: {},
    subtitle: {},
    invalidAccess: {},
    alertClass: {}
  },
  emits: ["done"],
  setup(e) {
    return (t, l) => {
      const s = z("EnsureAccess"), o = z("SlideOver");
      return e.invalidAccess ? (a(), X(o, {
        key: 0,
        title: e.title,
        onDone: l[0] || (l[0] = (r) => t.$emit("done")),
        "content-class": "relative flex-1"
      }, cn({
        default: xe(() => [
          he(s, {
            alertClass: e.alertClass,
            invalidAccess: e.invalidAccess
          }, null, 8, ["alertClass", "invalidAccess"])
        ]),
        _: 2
      }, [
        e.subtitle ? {
          name: "subtitle",
          fn: xe(() => [
            we(M(e.subtitle), 1)
          ]),
          key: "0"
        } : void 0
      ]), 1032, ["title"])) : x("", !0);
    };
  }
}), mu = ["for"], gu = ["type", "name", "id", "placeholder", "value", "aria-invalid", "aria-describedby"], pu = {
  key: 0,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, yu = ["id"], hu = ["id"], bu = {
  inheritAttrs: !1
}, wu = /* @__PURE__ */ ie({
  ...bu,
  __name: "TextInput",
  props: {
    status: {},
    id: {},
    type: {},
    inputClass: {},
    filterClass: { type: Function },
    label: {},
    labelClass: {},
    help: {},
    placeholder: {},
    modelValue: {}
  },
  setup(e, { expose: t }) {
    const l = (g) => sn(u.value, g.value), s = e;
    t({
      focus: r
    });
    const o = T();
    function r() {
      o.value?.focus();
    }
    const u = c(() => s.type || "text"), d = c(() => s.label ?? Pe(ot(s.id))), f = c(() => s.placeholder ?? d.value);
    function v(g) {
      return s.type === "range" ? g.replace("shadow-sm ", "") : g;
    }
    let y = ze("ApiState", void 0);
    const L = c(() => pt.call({ responseStatus: s.status ?? y?.error.value }, s.id)), w = c(() => kt([
      at.base,
      L.value ? at.invalid : v(at.valid),
      s.inputClass
    ], "TextInput", s.filterClass));
    return (g, h) => (a(), i("div", {
      class: m([g.$attrs.class])
    }, [
      H(g.$slots, "header", ke({
        inputElement: o.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, g.$attrs)),
      d.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, M(d.value), 11, mu)) : x("", !0),
      n("div", {
        class: m(v("mt-1 relative"))
      }, [
        n("input", ke({
          ref_key: "inputElement",
          ref: o,
          type: u.value,
          name: e.id,
          id: e.id,
          class: w.value,
          placeholder: f.value,
          value: W(sn)(u.value, e.modelValue),
          onInput: h[0] || (h[0] = (b) => g.$emit("update:modelValue", l(b.target))),
          "aria-invalid": L.value != null,
          "aria-describedby": `${e.id}-error`,
          step: "any"
        }, W(ft)(g.$attrs, ["class", "value"])), null, 16, gu),
        L.value ? (a(), i("div", pu, [...h[1] || (h[1] = [
          n("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "fill-rule": "evenodd",
              d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])])) : x("", !0)
      ], 2),
      L.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(L.value), 9, yu)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, M(e.help), 9, hu)) : x("", !0),
      H(g.$slots, "footer", ke({
        inputElement: o.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, g.$attrs))
    ], 2));
  }
}), xu = ["for"], ku = { class: "mt-1 relative" }, Cu = ["name", "id", "placeholder", "aria-invalid", "aria-describedby"], $u = ["id"], Lu = ["id"], Vu = {
  inheritAttrs: !1
}, Su = /* @__PURE__ */ ie({
  ...Vu,
  __name: "TextareaInput",
  props: {
    status: {},
    id: {},
    inputClass: {},
    filterClass: { type: Function },
    label: {},
    labelClass: {},
    help: {},
    placeholder: {},
    modelValue: {}
  },
  setup(e) {
    const t = (f) => f.value, l = e, s = c(() => l.label ?? Pe(ot(l.id))), o = c(() => l.placeholder ?? s.value);
    let r = ze("ApiState", void 0);
    const u = c(() => pt.call({ responseStatus: l.status ?? r?.error.value }, l.id)), d = c(() => kt([
      "shadow-sm " + at.base,
      u.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + at.valid,
      l.inputClass
    ], "TextareaInput", l.filterClass));
    return (f, v) => (a(), i("div", {
      class: m([f.$attrs.class])
    }, [
      s.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, M(s.value), 11, xu)) : x("", !0),
      n("div", ku, [
        n("textarea", ke({
          name: e.id,
          id: e.id,
          class: d.value,
          placeholder: o.value,
          onInput: v[0] || (v[0] = (y) => f.$emit("update:modelValue", t(y.target))),
          "aria-invalid": u.value != null,
          "aria-describedby": `${e.id}-error`
        }, W(ft)(f.$attrs, ["class"])), M(e.modelValue), 17, Cu)
      ]),
      u.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(u.value), 9, $u)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, M(e.help), 9, Lu)) : x("", !0)
    ], 2));
  }
}), Mu = ["for"], Tu = ["id", "name", "value", "aria-invalid", "aria-describedby"], Fu = ["value"], Au = ["id"], Iu = {
  inheritAttrs: !1
}, Du = /* @__PURE__ */ ie({
  ...Iu,
  __name: "SelectInput",
  props: {
    status: {},
    id: {},
    modelValue: {},
    inputClass: {},
    filterClass: { type: Function },
    label: {},
    labelClass: {},
    options: {},
    values: {},
    entries: {}
  },
  setup(e) {
    const t = (f) => f.value, l = e, s = c(() => l.label ?? Pe(ot(l.id)));
    let o = ze("ApiState", void 0);
    const r = c(() => pt.call({ responseStatus: l.status ?? o?.error.value }, l.id)), u = c(() => l.entries || (l.values ? l.values.map((f) => ({ key: f, value: f })) : l.options ? Object.keys(l.options).map((f) => ({ key: f, value: l.options[f] })) : [])), d = c(() => kt([
      "mt-1 block w-full pl-3 pr-10 py-2 text-base focus:outline-none sm:text-sm rounded-md dark:text-white dark:bg-gray-900 dark:border-gray-600 disabled:bg-slate-50 dark:disabled:bg-slate-900 disabled:text-slate-500 disabled:border-slate-200 dark:disabled:border-slate-700 disabled:shadow-none",
      r.value ? "border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500" : "shadow-sm border-gray-300 text-gray-900 focus:ring-indigo-500 focus:border-indigo-500",
      l.inputClass
    ], "SelectInput", l.filterClass));
    return (f, v) => (a(), i("div", {
      class: m([f.$attrs.class])
    }, [
      s.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, M(s.value), 11, Mu)) : x("", !0),
      n("select", ke({
        id: e.id,
        name: e.id,
        class: d.value,
        value: e.modelValue,
        onInput: v[0] || (v[0] = (y) => f.$emit("update:modelValue", t(y.target))),
        "aria-invalid": r.value != null,
        "aria-describedby": `${e.id}-error`
      }, W(ft)(f.$attrs, ["class"])), [
        (a(!0), i($e, null, Se(u.value, (y) => (a(), i("option", {
          value: y.key
        }, M(y.value), 9, Fu))), 256))
      ], 16, Tu),
      r.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(r.value), 9, Au)) : x("", !0)
    ], 2));
  }
}), Ou = { class: "flex items-center h-5" }, Pu = ["id", "name", "checked"], ju = { class: "ml-3 text-sm" }, Bu = ["for"], Hu = {
  key: 0,
  class: "mt-2 text-sm text-red-500",
  id: "`${id}-error`"
}, Ru = {
  key: 1,
  class: "mt-2 text-sm text-gray-500",
  id: "`${id}-description`"
}, Eu = {
  inheritAttrs: !1
}, Nu = /* @__PURE__ */ ie({
  ...Eu,
  __name: "CheckboxInput",
  props: {
    modelValue: { type: Boolean },
    status: {},
    id: {},
    inputClass: {},
    filterClass: { type: Function },
    label: {},
    labelClass: {},
    help: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, s = c(() => l.label ?? Pe(ot(l.id)));
    let o = ze("ApiState", void 0);
    const r = c(() => pt.call({ responseStatus: l.status ?? o?.error.value }, l.id)), u = c(() => kt(["focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800", l.inputClass], "CheckboxInput", l.filterClass));
    return (d, f) => (a(), i("div", {
      class: m(["relative flex items-start", d.$attrs.class])
    }, [
      n("div", Ou, [
        n("input", ke({
          id: e.id,
          name: e.id,
          type: "checkbox",
          checked: e.modelValue,
          onInput: f[0] || (f[0] = (v) => d.$emit("update:modelValue", v.target.checked)),
          class: u.value
        }, W(ft)(d.$attrs, ["class"])), null, 16, Pu)
      ]),
      n("div", ju, [
        n("label", {
          for: e.id,
          class: m(`font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, M(s.value), 11, Bu),
        r.value ? (a(), i("p", Hu, M(r.value), 1)) : e.help ? (a(), i("p", Ru, M(e.help), 1)) : x("", !0)
      ])
    ], 2));
  }
}), zu = ["id"], Uu = ["for"], qu = { class: "mt-1 relative" }, _u = ["id", "name", "value"], Qu = { class: "flex flex-wrap pb-1.5" }, Ku = { class: "pt-1.5 pl-1" }, Zu = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 dark:bg-indigo-800 text-indigo-700 dark:text-indigo-300" }, Wu = ["onClick"], Gu = { class: "pt-1.5 pl-1 shrink" }, Ju = ["type", "name", "id", "aria-invalid", "aria-describedby"], Xu = ["id"], Yu = ["onMouseover", "onClick"], ed = { class: "block truncate" }, td = {
  key: 1,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, ld = ["id"], nd = ["id"], sd = {
  inheritAttrs: !1
}, ad = /* @__PURE__ */ ie({
  ...sd,
  __name: "TagInput",
  props: {
    status: {},
    id: {},
    type: {},
    inputClass: {},
    filterClass: {},
    label: {},
    labelClass: {},
    help: {},
    modelValue: { default: () => [] },
    delimiters: { default: () => [","] },
    allowableValues: {},
    string: { type: Boolean },
    maxVisibleItems: { default: 300 },
    converter: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, s = t;
    function o(C) {
      return l.converter ? l.converter(C) : C;
    }
    const r = c(() => Ne(o(l.modelValue), (C) => typeof C == "string" ? C.trim().length == 0 ? [] : C.split(",") : C) || []), u = T(), d = T(!1), f = c(() => {
      const C = L.value.toLowerCase();
      return !l.allowableValues || l.allowableValues.length == 0 ? [] : l.allowableValues.length < 1e3 ? l.allowableValues.filter((P) => !r.value.includes(P) && P.toLowerCase().includes(C)) : l.allowableValues.filter((P) => !r.value.includes(P) && P.startsWith(C));
    });
    function v(C) {
      u.value = C;
    }
    const y = T(null), L = T(""), w = c(() => l.type || "text"), g = c(() => l.label ?? Pe(ot(l.id)));
    let h = ze("ApiState", void 0);
    const b = c(() => pt.call({ responseStatus: l.status ?? h?.error.value }, l.id)), A = c(() => kt([
      "w-full cursor-text flex flex-wrap sm:text-sm rounded-md dark:text-white dark:bg-gray-900 border focus-within:border-transparent focus-within:ring-1 focus-within:outline-none",
      b.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus-within:outline-none focus-within:ring-red-500 focus-within:border-red-500" : "shadow-sm border-gray-300 dark:border-gray-600 focus-within:ring-indigo-500 focus-within:border-indigo-500",
      l.inputClass
    ], "TagInput", l.filterClass)), B = (C) => S(r.value.filter((P) => P != C));
    function le(C) {
      document.activeElement === C.target && y.value?.focus();
    }
    const E = T();
    function I() {
      d.value = !0, E.value = !0;
    }
    function K() {
      I();
    }
    function U() {
      ne(ee()), E.value = !1, setTimeout(() => {
        E.value || (d.value = !1);
      }, 200);
    }
    function S(C) {
      const P = l.string ? C.join(",") : C;
      s("update:modelValue", P);
    }
    function V(C) {
      if (C.key == "Backspace" && L.value.length == 0 && r.value.length > 0 && B(r.value[r.value.length - 1]), !(!l.allowableValues || l.allowableValues.length == 0))
        if (C.code == "Escape" || C.code == "Tab")
          d.value = !1;
        else if (C.code == "Home")
          u.value = f.value[0], Z();
        else if (C.code == "End")
          u.value = f.value[f.value.length - 1], Z();
        else if (C.code == "ArrowDown") {
          if (d.value = !0, !u.value)
            u.value = f.value[0];
          else {
            const P = f.value.indexOf(u.value);
            u.value = P + 1 < f.value.length ? f.value[P + 1] : f.value[0];
          }
          _();
        } else if (C.code == "ArrowUp") {
          if (!u.value)
            u.value = f.value[f.value.length - 1];
          else {
            const P = f.value.indexOf(u.value);
            u.value = P - 1 >= 0 ? f.value[P - 1] : f.value[f.value.length - 1];
          }
          _();
        } else C.code == "Enter" ? u.value && d.value ? (ne(u.value), C.preventDefault()) : d.value = !1 : d.value = f.value.length > 0;
    }
    function ee() {
      if (L.value.length == 0) return "";
      let C = Aa(L.value.trim(), ",");
      return C[0] == "," && (C = C.substring(1)), C = C.trim(), C.length == 0 && d.value && f.value.length > 0 ? u.value : C;
    }
    function re(C) {
      const P = ee();
      if (P.length > 0) {
        const Q = l.delimiters.some((p) => p == C.key);
        if (Q && C.preventDefault(), C.key == "Enter" || C.key == "NumpadEnter" || C.key.length == 1 && Q) {
          ne(P);
          return;
        }
      }
    }
    const O = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function Z() {
      setTimeout(() => {
        let C = $l(`#${l.id}-tag li.active`);
        C && C.scrollIntoView(O);
      }, 0);
    }
    function _() {
      setTimeout(() => {
        let C = $l(`#${l.id}-tag li.active`);
        C && ("scrollIntoViewIfNeeded" in C ? C.scrollIntoViewIfNeeded(O) : C.scrollIntoView(O));
      }, 0);
    }
    function ne(C) {
      if (C.length === 0) return;
      const P = Array.from(r.value);
      P.indexOf(C) == -1 && P.push(C), S(P), L.value = "", d.value = !1;
    }
    function F(C) {
      const P = C.clipboardData?.getData("Text");
      J(P);
    }
    function J(C) {
      if (!C) return;
      const P = new RegExp(`\\n|\\t|${l.delimiters.join("|")}`), Q = Array.from(r.value);
      C.split(P).map((p) => p.trim()).forEach((p) => {
        Q.indexOf(p) == -1 && Q.push(p);
      }), S(Q), L.value = "";
    }
    return (C, P) => (a(), i("div", {
      class: m([C.$attrs.class]),
      id: `${e.id}-tag`,
      onmousemove: "cancelBlur=true"
    }, [
      g.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, M(g.value), 11, Uu)) : x("", !0),
      n("div", qu, [
        n("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: r.value.join(",")
        }, null, 8, _u),
        n("button", {
          class: m(A.value),
          onClick: He(le, ["prevent"]),
          onFocus: P[2] || (P[2] = (Q) => d.value = !0),
          tabindex: "-1"
        }, [
          n("div", Qu, [
            (a(!0), i($e, null, Se(r.value, (Q) => (a(), i("div", Ku, [
              n("span", Zu, [
                we(M(Q) + " ", 1),
                n("button", {
                  type: "button",
                  onClick: (k) => B(Q),
                  class: "flex-shrink-0 ml-1 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 dark:text-indigo-500 hover:bg-indigo-200 dark:hover:bg-indigo-800 hover:text-indigo-500 dark:hover:text-indigo-400 focus:outline-none focus:bg-indigo-500 focus:text-white dark:focus:text-black"
                }, [...P[3] || (P[3] = [
                  n("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    n("path", {
                      "stroke-linecap": "round",
                      "stroke-width": "1.5",
                      d: "M1 1l6 6m0-6L1 7"
                    })
                  ], -1)
                ])], 8, Wu)
              ])
            ]))), 256)),
            n("div", Gu, [
              It(n("input", ke({
                ref_key: "txtInput",
                ref: y,
                type: w.value,
                role: "combobox",
                "aria-controls": "options",
                "aria-expanded": "false",
                autocomplete: "off",
                spellcheck: "false",
                name: `${e.id}-txt`,
                id: `${e.id}-txt`,
                class: "p-0 dark:bg-transparent rounded-md border-none focus:!border-none focus:!outline-none",
                style: `box-shadow:none !important;width:${L.value.length + 1}ch`,
                "onUpdate:modelValue": P[0] || (P[0] = (Q) => L.value = Q),
                "aria-invalid": b.value != null,
                "aria-describedby": `${e.id}-error`,
                onKeydown: V,
                onKeypress: re,
                onPaste: He(F, ["prevent", "stop"]),
                onFocus: K,
                onBlur: U,
                onClick: P[1] || (P[1] = (Q) => d.value = !0)
              }, W(ft)(C.$attrs, ["class", "required"])), null, 16, Ju), [
                [ma, L.value]
              ])
            ])
          ])
        ], 34),
        d.value && f.value.length ? (a(), i("ul", {
          key: 0,
          class: "absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm",
          onKeydown: V,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), i($e, null, Se(f.value.slice(0, e.maxVisibleItems), (Q) => (a(), i("li", {
            class: m([Q === u.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (k) => v(Q),
            onClick: (k) => ne(Q),
            role: "option",
            tabindex: "-1"
          }, [
            n("span", ed, M(Q), 1)
          ], 42, Yu))), 256))
        ], 40, Xu)) : x("", !0),
        b.value ? (a(), i("div", td, [...P[4] || (P[4] = [
          n("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "fill-rule": "evenodd",
              d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])])) : x("", !0)
      ]),
      b.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(b.value), 9, ld)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, M(e.help), 9, nd)) : x("", !0)
    ], 10, zu));
  }
}), od = { class: "relative flex-grow mr-2 sm:mr-4" }, id = ["for"], rd = { class: "block mt-2" }, ud = { class: "sr-only" }, dd = ["multiple", "name", "id", "placeholder", "aria-invalid", "aria-describedby"], cd = {
  key: 0,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, fd = ["id"], vd = ["id"], md = { key: 0 }, gd = ["title"], pd = ["alt", "src"], yd = {
  key: 1,
  class: "mt-3"
}, hd = { class: "w-full" }, bd = { class: "pr-6 align-bottom pb-2" }, wd = ["title"], xd = ["src", "onError"], kd = ["href"], Cd = {
  key: 1,
  class: "overflow-hidden"
}, $d = { class: "align-top pb-2 whitespace-nowrap" }, Ld = {
  key: 0,
  class: "text-gray-500 dark:text-gray-400 text-sm bg-white dark:bg-black"
}, Vd = /* @__PURE__ */ ie({
  __name: "FileInput",
  props: {
    multiple: { type: Boolean },
    status: {},
    id: {},
    inputClass: {},
    filterClass: { type: Function },
    label: {},
    labelClass: {},
    help: {},
    placeholder: {},
    modelValue: {},
    values: {},
    files: {}
  },
  setup(e) {
    const t = e, l = T(null), { assetsPathResolver: s, fallbackPathResolver: o } = Bt(), r = {}, u = T(), d = T(t.files?.map(f) || []);
    function f(I) {
      return I.filePath = s(I.filePath), I;
    }
    t.values && t.values.length > 0 && (d.value = t.values.map((I) => {
      let K = I.replace(/\\/g, "/");
      return { fileName: us(Dt(K, "/"), "."), filePath: K, contentType: en(K) };
    }).map(f));
    const v = c(() => t.label ?? Pe(ot(t.id))), y = c(() => t.placeholder ?? v.value);
    let L = ze("ApiState", void 0);
    const w = c(() => pt.call({ responseStatus: t.status ?? L?.error.value }, t.id)), g = c(() => kt([
      "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-violet-50 dark:file:bg-violet-900 file:text-violet-700 dark:file:text-violet-200 hover:file:bg-violet-100 dark:hover:file:bg-violet-800",
      w.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500" : "text-slate-500 dark:text-slate-400",
      t.inputClass
    ], "FileInput", t.filterClass)), h = (I) => {
      let K = I.target;
      u.value = "", d.value = Array.from(K.files || []).map((U) => ({
        fileName: U.name,
        filePath: gn(U),
        contentLength: U.size,
        contentType: U.type || en(U.name)
      }));
    }, b = () => l.value?.click(), A = (I) => I == null ? !1 : I.startsWith("data:") || I.startsWith("blob:"), B = c(() => {
      if (d.value.length > 0)
        return d.value[0].filePath;
      let I = typeof t.modelValue == "string" ? t.modelValue : t.values && t.values[0];
      return I && St(s(I)) || null;
    }), le = (I) => !I || I.startsWith("data:") || I.endsWith(".svg") ? "" : "rounded-full object-cover";
    function E(I) {
      u.value = o(B.value);
    }
    return jt(ps), (I, K) => (a(), i("div", {
      class: m(["flex", e.multiple ? "flex-col" : "justify-between"])
    }, [
      n("div", od, [
        v.value ? (a(), i("label", {
          key: 0,
          for: e.id,
          class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, M(v.value), 11, id)) : x("", !0),
        n("div", rd, [
          n("span", ud, M(e.help ?? v.value), 1),
          n("input", ke({
            ref_key: "input",
            ref: l,
            type: "file",
            multiple: e.multiple,
            name: e.id,
            id: e.id,
            class: g.value,
            placeholder: y.value,
            "aria-invalid": w.value != null,
            "aria-describedby": `${e.id}-error`
          }, I.$attrs, { onChange: h }), null, 16, dd),
          w.value ? (a(), i("div", cd, [...K[0] || (K[0] = [
            n("svg", {
              class: "h-5 w-5 text-red-500",
              xmlns: "http://www.w3.org/2000/svg",
              viewBox: "0 0 20 20",
              fill: "currentColor",
              "aria-hidden": "true"
            }, [
              n("path", {
                "fill-rule": "evenodd",
                d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
                "clip-rule": "evenodd"
              })
            ], -1)
          ])])) : x("", !0)
        ]),
        w.value ? (a(), i("p", {
          key: 1,
          class: "mt-2 text-sm text-red-500",
          id: `${e.id}-error`
        }, M(w.value), 9, fd)) : e.help ? (a(), i("p", {
          key: 2,
          class: "mt-2 text-sm text-gray-500",
          id: `${e.id}-description`
        }, M(e.help), 9, vd)) : x("", !0)
      ]),
      e.multiple ? (a(), i("div", yd, [
        n("table", hd, [
          (a(!0), i($e, null, Se(d.value, (U) => (a(), i("tr", null, [
            n("td", bd, [
              n("div", {
                class: "flex w-full",
                title: A(U.filePath) ? "" : U.filePath
              }, [
                n("img", {
                  src: r[W(St)(U.filePath)] || W(s)(W(St)(U.filePath)),
                  class: m(["mr-2 h-8 w-8", le(U.filePath)]),
                  onError: (S) => r[W(St)(U.filePath)] = W(o)(W(St)(U.filePath))
                }, null, 42, xd),
                A(U.filePath) ? (a(), i("span", Cd, M(U.fileName), 1)) : (a(), i("a", {
                  key: 0,
                  href: W(s)(U.filePath || ""),
                  target: "_blank",
                  class: "overflow-hidden"
                }, M(U.fileName), 9, kd))
              ], 8, wd)
            ]),
            n("td", $d, [
              U.contentLength && U.contentLength > 0 ? (a(), i("span", Ld, M(W(yn)(U.contentLength)), 1)) : x("", !0)
            ])
          ]))), 256))
        ])
      ])) : (a(), i("div", md, [
        B.value ? (a(), i("div", {
          key: 0,
          class: "shrink-0 cursor-pointer",
          title: A(B.value) ? "" : B.value
        }, [
          n("img", {
            onClick: b,
            class: m(["h-16 w-16", le(B.value)]),
            alt: `Current ${v.value ?? ""}`,
            src: u.value || W(s)(B.value),
            onError: E
          }, null, 42, pd)
        ], 8, gd)) : x("", !0)
      ]))
    ], 2));
  }
}), Sd = ["id"], Md = ["for"], Td = { class: "relative mt-1" }, Fd = ["id", "placeholder"], Ad = ["id"], Id = ["onMouseover", "onClick"], Dd = {
  key: 2,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none",
  tabindex: "-1"
}, Od = ["id"], Pd = ["id"], jd = /* @__PURE__ */ ie({
  __name: "Autocomplete",
  props: {
    status: {},
    id: {},
    type: {},
    label: {},
    help: {},
    placeholder: {},
    multiple: { type: Boolean, default: !1 },
    required: { type: Boolean },
    options: { default: () => [] },
    modelValue: {},
    match: {},
    viewCount: { default: 100 },
    pageSize: { default: 8 }
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const s = T(!1), o = e, r = l;
    t({ toggle: O });
    function u(F) {
      return Array.isArray(o.modelValue) && o.modelValue.indexOf(F) >= 0;
    }
    const d = c(() => o.label ?? Pe(ot(o.id)));
    let f = ze("ApiState", void 0);
    const v = c(() => pt.call({ responseStatus: o.status ?? f?.error.value }, o.id)), y = c(() => [at.base, v.value ? at.invalid : at.valid]), L = T(null), w = T(""), g = T(null), h = T(o.viewCount), b = T([]), A = c(() => w.value ? o.options.filter((J) => o.match(J, w.value)).slice(0, h.value) : o.options), B = ["Tab", "Escape", "ArrowDown", "ArrowUp", "Enter", "PageUp", "PageDown", "Home", "End"];
    function le(F) {
      g.value = F, b.value.indexOf(F) > Math.floor(h.value * 0.9) && (h.value += o.viewCount, ne());
    }
    const E = [",", `
`, "	"];
    function I(F) {
      const J = F.clipboardData?.getData("Text");
      K(J);
    }
    function K(F) {
      if (!F) return;
      const J = E.some((C) => F.includes(C));
      if (!o.multiple || !J) {
        const C = o.options.filter((P) => o.match(P, F));
        C.length == 1 && (_(C[0]), s.value = !1, xl());
      } else if (J) {
        const C = new RegExp("\\r|\\n|\\t|,"), Q = F.split(C).filter((k) => k.trim()).map((k) => o.options.find((p) => o.match(p, k))).filter((k) => !!k);
        if (Q.length > 0) {
          w.value = "", s.value = !1, g.value = null;
          let k = Array.from(o.modelValue || []);
          Q.forEach((p) => {
            u(p) ? k = k.filter((te) => te != p) : k.push(p);
          }), r("update:modelValue", k), xl();
        }
      }
    }
    function U(F) {
      B.indexOf(F.code) || Z();
    }
    function S(F) {
      if (!(F.shiftKey || F.ctrlKey || F.altKey)) {
        if (!s.value) {
          F.code == "ArrowDown" && (s.value = !0, g.value = b.value[0]);
          return;
        }
        if (F.code == "Escape")
          s.value && (F.stopPropagation(), s.value = !1);
        else if (F.code == "Tab")
          s.value = !1;
        else if (F.code == "Home")
          g.value = b.value[0], ee();
        else if (F.code == "End")
          g.value = b.value[b.value.length - 1], ee();
        else if (F.code == "ArrowDown") {
          if (!g.value)
            g.value = b.value[0];
          else {
            const J = b.value.indexOf(g.value);
            g.value = J + 1 < b.value.length ? b.value[J + 1] : b.value[0];
          }
          re();
        } else if (F.code == "ArrowUp") {
          if (!g.value)
            g.value = b.value[b.value.length - 1];
          else {
            const J = b.value.indexOf(g.value);
            g.value = J - 1 >= 0 ? b.value[J - 1] : b.value[b.value.length - 1];
          }
          re();
        } else F.code == "Enter" && (g.value ? (_(g.value), o.multiple || (F.preventDefault(), xl())) : s.value = !1);
      }
    }
    const V = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function ee() {
      setTimeout(() => {
        let F = $l(`#${o.id}-autocomplete li.active`);
        F && F.scrollIntoView(V);
      }, 0);
    }
    function re() {
      setTimeout(() => {
        let F = $l(`#${o.id}-autocomplete li.active`);
        F && ("scrollIntoViewIfNeeded" in F ? F.scrollIntoViewIfNeeded(V) : F.scrollIntoView(V));
      }, 0);
    }
    function O(F) {
      s.value = F, F && (Z(), L.value?.focus());
    }
    function Z() {
      s.value = !0, ne();
    }
    function _(F) {
      if (w.value = "", s.value = !1, o.multiple) {
        let J = Array.from(o.modelValue || []);
        u(F) ? J = J.filter((C) => C != F) : J.push(F), g.value = null, r("update:modelValue", J);
      } else {
        let J = F;
        o.modelValue == F && (J = null), r("update:modelValue", J);
      }
    }
    function ne() {
      b.value = A.value;
    }
    return xt(w, ne), (F, J) => (a(), i("div", {
      id: `${e.id}-autocomplete`
    }, [
      d.value ? (a(), i("label", {
        key: 0,
        for: `${e.id}-text`,
        class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
      }, M(d.value), 9, Md)) : x("", !0),
      n("div", Td, [
        It(n("input", ke({
          ref_key: "txtInput",
          ref: L,
          id: `${e.id}-text`,
          type: "text",
          role: "combobox",
          "aria-controls": "options",
          "aria-expanded": "false",
          autocomplete: "off",
          spellcheck: "false",
          "onUpdate:modelValue": J[0] || (J[0] = (C) => w.value = C),
          class: y.value,
          placeholder: e.multiple || !e.modelValue ? e.placeholder : "",
          onFocus: Z,
          onKeydown: S,
          onKeyup: U,
          onClick: Z,
          onPaste: I,
          required: !1
        }, F.$attrs), null, 16, Fd), [
          [ga, w.value]
        ]),
        n("button", {
          type: "button",
          onClick: J[1] || (J[1] = (C) => O(!s.value)),
          class: "absolute inset-y-0 right-0 flex items-center rounded-r-md px-2 focus:outline-none",
          tabindex: "-1"
        }, [...J[3] || (J[3] = [
          n("svg", {
            class: "h-5 w-5 text-gray-400 dark:text-gray-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "fill-rule": "evenodd",
              d: "M10 3a.75.75 0 01.55.24l3.25 3.5a.75.75 0 11-1.1 1.02L10 4.852 7.3 7.76a.75.75 0 01-1.1-1.02l3.25-3.5A.75.75 0 0110 3zm-3.76 9.2a.75.75 0 011.06.04l2.7 2.908 2.7-2.908a.75.75 0 111.1 1.02l-3.25 3.5a.75.75 0 01-1.1 0l-3.25-3.5a.75.75 0 01.04-1.06z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])]),
        s.value ? (a(), i("ul", {
          key: 0,
          class: "absolute z-20 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm",
          onKeydown: S,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), i($e, null, Se(b.value, (C) => (a(), i("li", {
            class: m([C === g.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (P) => le(C),
            onClick: (P) => _(C),
            role: "option",
            tabindex: "-1"
          }, [
            typeof C == "string" ? H(F.$slots, "item", ke({
              key: 0,
              ref_for: !0
            }, { key: C, value: C })) : H(F.$slots, "item", ke({
              key: 1,
              ref_for: !0
            }, C)),
            u(C) ? (a(), i("span", {
              key: 2,
              class: m(["absolute inset-y-0 right-0 flex items-center pr-4", C === g.value ? "text-white" : "text-indigo-600"])
            }, [...J[4] || (J[4] = [
              n("svg", {
                class: "h-5 w-5",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                n("path", {
                  "fill-rule": "evenodd",
                  d: "M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z",
                  "clip-rule": "evenodd"
                })
              ], -1)
            ])], 2)) : x("", !0)
          ], 42, Id))), 256))
        ], 40, Ad)) : !e.multiple && e.modelValue ? (a(), i("div", {
          key: 1,
          onKeydown: S,
          onClick: J[2] || (J[2] = (C) => O(!s.value)),
          class: "h-8 -mt-8 ml-3 pt-0.5"
        }, [
          typeof e.modelValue == "string" ? H(F.$slots, "item", Cl(ke({ key: 0 }, { key: e.modelValue, value: e.modelValue }))) : H(F.$slots, "item", Cl(ke({ key: 1 }, e.modelValue)))
        ], 32)) : x("", !0),
        v.value ? (a(), i("div", Dd, [...J[5] || (J[5] = [
          n("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "fill-rule": "evenodd",
              d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])])) : x("", !0)
      ]),
      v.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(v.value), 9, Od)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, M(e.help), 9, Pd)) : x("", !0)
    ], 8, Sd));
  }
}), Bd = ["id", "name", "value"], Hd = { class: "block truncate" }, Rd = /* @__PURE__ */ ie({
  __name: "Combobox",
  props: {
    id: {},
    modelValue: {},
    multiple: { type: Boolean },
    options: {},
    values: {},
    entries: {}
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const s = e;
    t({
      toggle(g) {
        d.value?.toggle(g);
      }
    });
    const o = l;
    function r(g) {
      o("update:modelValue", g);
    }
    const u = c(() => s.multiple != null ? s.multiple : Array.isArray(s.modelValue)), d = T();
    function f(g, h) {
      return !h || g.value.toLowerCase().includes(h.toLowerCase());
    }
    const v = c(() => s.entries || (s.values ? s.values.map((g) => ({ key: g, value: g })) : s.options ? Object.keys(s.options).map((g) => ({ key: g, value: s.options[g] })) : [])), y = T(u.value ? [] : null);
    function L() {
      let g = s.modelValue && typeof s.modelValue == "object" && !Array.isArray(s.modelValue) ? s.modelValue.key : s.modelValue;
      g == null || g === "" ? y.value = u.value ? [] : null : typeof g == "string" ? y.value = v.value.find((h) => h.key === g) || null : Array.isArray(g) && (y.value = v.value.filter((h) => g.includes(h.key)));
    }
    Ye(L);
    const w = c(() => y.value == null ? "" : Array.isArray(y.value) ? y.value.map((g) => encodeURIComponent(g.key)).join(",") : y.value.key);
    return (g, h) => {
      const b = z("Autocomplete");
      return a(), i($e, null, [
        n("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: w.value
        }, null, 8, Bd),
        he(b, ke({
          ref_key: "input",
          ref: d,
          id: e.id,
          options: v.value,
          match: f,
          multiple: u.value
        }, g.$attrs, {
          modelValue: y.value,
          "onUpdate:modelValue": [
            h[0] || (h[0] = (A) => y.value = A),
            r
          ]
        }), {
          item: xe(({ key: A, value: B }) => [
            n("span", Hd, M(B), 1)
          ]),
          _: 1
        }, 16, ["id", "options", "multiple", "modelValue"])
      ], 64);
    };
  }
}), Ed = /* @__PURE__ */ ie({
  __name: "DynamicInput",
  props: {
    input: {},
    modelValue: {},
    api: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, s = t, o = c(() => l.input.type || "text"), r = "ignore,css,options,meta,allowableValues,allowableEntries,op,prop,type,id,name".split(","), u = c(() => ft(l.input, r)), d = T(o.value === "file" ? null : l.modelValue[l.input.id]);
    xt(d, () => {
      l.modelValue[l.input.id] = d.value, s("update:modelValue", l.modelValue);
    });
    const f = c(() => {
      const v = l.modelValue[l.input.id];
      if (l.input.type !== "file" || !v) return [];
      if (typeof v == "string") return [{ filePath: v, fileName: Dt(v, "/") }];
      if (!Array.isArray(v) && typeof v == "object") return v;
      if (Array.isArray(v)) {
        const y = [];
        return v.forEach((L) => {
          typeof L == "string" ? y.push({ filePath: L, fileName: Dt(L, "/") }) : typeof L == "object" && y.push(L);
        }), y;
      }
    });
    return (v, y) => {
      const L = z("SelectInput"), w = z("CheckboxInput"), g = z("TagInput"), h = z("Combobox"), b = z("FileInput"), A = z("TextareaInput"), B = z("MarkdownInput"), le = z("TextInput");
      return W(q).component(o.value) ? (a(), X(is(W(q).component(o.value)), ke({
        key: 0,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[0] || (y[0] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "select" ? (a(), X(L, ke({
        key: 1,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[1] || (y[1] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o.value == "checkbox" ? (a(), X(w, ke({
        key: 2,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[2] || (y[2] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "tag" ? (a(), X(g, ke({
        key: 3,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[3] || (y[3] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        allowableValues: e.input.allowableValues,
        string: e.input.prop?.type == "String"
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "allowableValues", "string"])) : o.value == "combobox" ? (a(), X(h, ke({
        key: 4,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[4] || (y[4] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o.value == "file" ? (a(), X(b, ke({
        key: 5,
        id: e.input.id,
        status: e.api?.error,
        modelValue: d.value,
        "onUpdate:modelValue": y[5] || (y[5] = (E) => d.value = E),
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        files: f.value
      }, u.value), null, 16, ["id", "status", "modelValue", "input-class", "label-class", "files"])) : o.value == "textarea" ? (a(), X(A, ke({
        key: 6,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[6] || (y[6] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "MarkdownInput" ? (a(), X(B, ke({
        key: 7,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[7] || (y[7] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : (a(), X(le, ke({
        key: 8,
        type: o.value,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": y[8] || (y[8] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["type", "id", "modelValue", "status", "input-class", "label-class"]));
    };
  }
}), Nd = { class: "lookup-field" }, zd = ["name", "value"], Ud = {
  key: 0,
  class: "flex justify-between"
}, qd = ["for"], _d = {
  key: 0,
  class: "flex items-center"
}, Qd = { class: "text-sm text-gray-500 dark:text-gray-400 pr-1" }, Kd = {
  key: 1,
  class: "mt-1 relative"
}, Zd = { class: "w-full inline-flex truncate" }, Wd = { class: "text-blue-700 dark:text-blue-300 flex cursor-pointer" }, Gd = ["id"], Jd = ["id"], Xd = /* @__PURE__ */ ie({
  __name: "LookupInput",
  props: {
    id: {},
    status: {},
    input: {},
    metadataType: {},
    modelValue: {},
    label: {},
    labelClass: {},
    help: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const { config: l } = Bt(), { metadataApi: s } = lt(), o = e, r = t, u = c(() => o.id || o.input.id), d = c(() => o.label ?? Pe(ot(u.value)));
    let f = ze("ApiState", void 0);
    const v = ze("client"), y = c(() => pt.call({ responseStatus: o.status ?? f?.error.value }, u.value)), L = T(""), w = T(""), g = c(() => be(o.modelValue, u.value)), h = c(() => Xe(o.metadataType).find((K) => K.name.toLowerCase() == u.value.toLowerCase())), b = c(() => it(h.value?.ref?.model)?.icon || l.value.tableIcon);
    function A(K) {
      return K ? o.input.options ? Object.assign({}, K, Hl(o.input.options, {
        input: o.input,
        $typeFields: Xe(o.metadataType).map((U) => U.name),
        ...q.config.scopeWhitelist
      })) : K : null;
    }
    const B = c(() => A(h.value?.ref ?? (o.input.type == "lookup" ? {
      model: o.metadataType.name,
      refId: Wt(o.metadataType)?.name ?? "id",
      refLabel: o.metadataType.properties?.find((K) => K.type == "String" && !K.isPrimaryKey)?.name
    } : null)));
    let le;
    function E(K) {
      if (K) {
        if (le == null) {
          console.warn("No ModalProvider required by LookupInput");
          return;
        }
        le.openModal({ name: "ModalLookup", ref: K }, (U) => {
          if (console.debug("openModal", L.value, " -> ", U, Ze.setRefValue(K, U), K), U) {
            const S = be(U, K.refId);
            L.value = Ze.setRefValue(K, U) || S;
            const V = W(o.modelValue);
            V[u.value] = S, r("update:modelValue", V);
          }
        });
      }
    }
    function I() {
      o.modelValue[u.value] = null, L.value = "";
    }
    return Ye(async () => {
      le = ze("ModalProvider", void 0);
      const K = o.modelValue;
      o.modelValue[u.value] || (o.modelValue[u.value] = null);
      const U = h.value, S = B.value;
      if (!U || !S) {
        console.warn(`No RefInfo for property '${u.value}'`);
        return;
      }
      L.value = "";
      let V = S.selfId == null ? be(K, U.name) : be(K, S.selfId);
      if (il(V) && (V = be(K, S.refId)), V == null)
        return;
      const re = s.value?.operations.find((O) => O.dataModel?.name == S.model);
      if (console.debug("LookupInput queryOp", re), re != null) {
        const O = be(K, U.name);
        if (il(O)) return;
        if (L.value = `${O}`, w.value = U.name, S.refLabel != null) {
          const Z = Xe(o.metadataType).filter((F) => F.type == S.model);
          Z.length || console.warn(`Could not find ${S.model} Property on ${o.metadataType.name}`);
          const _ = Z.map((F) => be(K, F.name)).filter((F) => !!F), ne = _.length <= 1 ? _[0] : _.find((F) => F[S.refId ?? "id"] == V);
          if (ne != null) {
            let F = be(ne, S.refLabel);
            F && (L.value = `${F}`, Ze.setValue(S.model, V, S.refLabel, F));
          } else {
            const F = U.attributes?.some((C) => C.name == "Computed") == !0;
            let J = await Ze.getOrFetchValue(v, s.value, S.model, S.refId, S.refLabel, F, V);
            L.value = J || `${S.model}: ${L.value}`;
          }
        }
      }
    }), (K, U) => {
      const S = z("Icon");
      return a(), i("div", Nd, [
        n("input", {
          type: "hidden",
          name: u.value,
          value: g.value
        }, null, 8, zd),
        d.value ? (a(), i("div", Ud, [
          n("label", {
            for: u.value,
            class: m(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
          }, M(d.value), 11, qd),
          g.value ? (a(), i("div", _d, [
            n("span", Qd, M(g.value), 1),
            n("button", {
              onClick: I,
              type: "button",
              title: "clear",
              class: "mr-1 rounded-md text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
            }, [...U[1] || (U[1] = [
              n("span", { class: "sr-only" }, "Clear", -1),
              n("svg", {
                class: "h-4 w-4",
                xmlns: "http://www.w3.org/2000/svg",
                fill: "none",
                viewBox: "0 0 24 24",
                "stroke-width": "1.5",
                stroke: "currentColor",
                "aria-hidden": "true"
              }, [
                n("path", {
                  "stroke-linecap": "round",
                  "stroke-linejoin": "round",
                  d: "M6 18L18 6M6 6l12 12"
                })
              ], -1)
            ])])
          ])) : x("", !0)
        ])) : x("", !0),
        B.value ? (a(), i("div", Kd, [
          n("button", {
            type: "button",
            class: "lookup flex relative w-full bg-white dark:bg-black border border-gray-300 dark:border-gray-700 rounded-md shadow-sm pl-3 pr-10 py-2 text-left focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm",
            onClick: U[0] || (U[0] = (V) => E(B.value)),
            "aria-haspopup": "listbox",
            "aria-expanded": "true",
            "aria-labelledby": "listbox-label"
          }, [
            n("span", Zd, [
              n("span", Wd, [
                he(S, {
                  class: "mr-1 w-5 h-5",
                  image: b.value
                }, null, 8, ["image"]),
                n("span", null, M(L.value), 1)
              ])
            ]),
            U[2] || (U[2] = n("span", { class: "absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none" }, [
              n("svg", {
                class: "h-5 w-5 text-gray-400 dark:text-gray-500",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                n("path", {
                  "fill-rule": "evenodd",
                  d: "M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z",
                  "clip-rule": "evenodd"
                })
              ])
            ], -1))
          ])
        ])) : x("", !0),
        y.value ? (a(), i("p", {
          key: 2,
          class: "mt-2 text-sm text-red-500",
          id: `${u.value}-error`
        }, M(y.value), 9, Gd)) : e.help ? (a(), i("p", {
          key: 3,
          class: "mt-2 text-sm text-gray-500",
          id: `${u.value}-description`
        }, M(e.help), 9, Jd)) : x("", !0)
      ]);
    };
  }
}), Yd = /* @__PURE__ */ ie({
  __name: "AutoFormFields",
  props: {
    modelValue: {},
    type: {},
    metaType: {},
    api: {},
    formLayout: {},
    configureField: {},
    configureFormLayout: {},
    hideSummary: { type: Boolean },
    flexClass: { default: "flex flex-1 flex-col justify-between" },
    divideClass: { default: "divide-y divide-gray-200 px-4 sm:px-6" },
    spaceClass: { default: "space-y-6 pt-6 pb-5" },
    fieldsetClass: { default: "grid grid-cols-12 gap-6" }
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const s = e, o = l;
    t({ forceUpdate: r, props: s, updateValue: d });
    function r() {
      Oe()?.proxy?.$forceUpdate();
    }
    function u(E, I) {
      d(E.id, be(I, E.id));
    }
    function d(E, I) {
      s.modelValue[E] = I, o("update:modelValue", s.modelValue), r();
    }
    const { metadataApi: f, apiOf: v, typeOf: y, typeOfRef: L, createFormLayout: w, Crud: g } = lt(), h = c(() => s.type || Ht(s.modelValue)), b = c(() => s.metaType ?? y(h.value)), A = c(() => L(f.value?.operations.find((E) => E.request.name == h.value)?.dataModel) || b.value);
    function B() {
      const E = b.value;
      if (!E) {
        if (s.formLayout) {
          const ee = s.formLayout.map((re) => {
            const O = { name: re.id, type: _a(re.type) }, Z = Object.assign({ prop: O }, re);
            return s.configureField && s.configureField(Z), Z;
          });
          return s.configureFormLayout && s.configureFormLayout(ee), ee;
        }
        throw new Error(`MetadataType for ${h.value} not found`);
      }
      const I = Xe(E), K = A.value, U = s.formLayout ? Array.from(s.formLayout) : w(E), S = [], V = v(E.name);
      return U.forEach((ee) => {
        const re = I.find((_) => _.name == ee.name);
        if (ee.ignore) return;
        const O = K?.properties?.find((_) => _.name.toLowerCase() == ee.name?.toLowerCase()) ?? re, Z = Object.assign({ prop: O, op: V }, ee);
        s.configureField && s.configureField(Z), S.push(Z);
      }), s.configureFormLayout && s.configureFormLayout(S), S;
    }
    const le = () => B().filter((E) => E.type != "hidden").map((E) => E.id);
    return (E, I) => {
      const K = z("ErrorSummary"), U = z("LookupInput"), S = z("DynamicInput");
      return a(), i($e, null, [
        e.hideSummary ? x("", !0) : (a(), X(K, {
          key: 0,
          status: e.api?.error,
          except: le()
        }, null, 8, ["status", "except"])),
        n("div", {
          class: m(e.flexClass)
        }, [
          n("div", {
            class: m(e.divideClass)
          }, [
            n("div", {
              class: m(e.spaceClass)
            }, [
              n("fieldset", {
                class: m(e.fieldsetClass)
              }, [
                (a(!0), i($e, null, Se(B(), (V) => (a(), i("div", {
                  key: V.id,
                  class: m([
                    "w-full",
                    V.css?.field ?? (V.type == "textarea" ? "col-span-12" : "col-span-12 xl:col-span-6" + (V.type == "checkbox" ? " flex items-center" : "")),
                    V.type == "hidden" ? "hidden" : ""
                  ])
                }, [
                  V.type === "lookup" || V.prop?.ref != null && V.type != "file" && !V.prop.isPrimaryKey ? (a(), X(U, {
                    key: 0,
                    metadataType: A.value,
                    input: V,
                    modelValue: e.modelValue,
                    "onUpdate:modelValue": (ee) => u(V, ee),
                    status: e.api?.error
                  }, null, 8, ["metadataType", "input", "modelValue", "onUpdate:modelValue", "status"])) : (a(), X(S, {
                    key: 1,
                    input: V,
                    modelValue: e.modelValue,
                    "onUpdate:modelValue": I[0] || (I[0] = (ee) => E.$emit("update:modelValue", ee)),
                    api: e.api
                  }, null, 8, ["input", "modelValue", "api"]))
                ], 2))), 128))
              ], 2)
            ], 2)
          ], 2)
        ], 2)
      ], 64);
    };
  }
}), ec = { key: 0 }, tc = { class: "text-red-700" }, lc = { key: 0 }, nc = { key: 2 }, sc = ["innerHTML"], ac = { class: "flex justify-end" }, oc = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, ic = { class: "fixed inset-0 overflow-hidden" }, rc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, uc = { class: "flex-1" }, dc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, cc = { class: "flex items-start justify-between space-x-3" }, fc = { class: "space-y-1" }, vc = { key: 0 }, mc = { key: 2 }, gc = ["innerHTML"], pc = { class: "flex h-7 items-center" }, yc = { class: "flex justify-end" }, hc = /* @__PURE__ */ ie({
  __name: "AutoForm",
  props: {
    type: {},
    modelValue: {},
    heading: {},
    subHeading: {},
    showLoading: { type: Boolean, default: !0 },
    jsconfig: { default: "eccn,edv" },
    formStyle: { default: "card" },
    metaType: {},
    configureField: {},
    configureFormLayout: {},
    panelClass: {},
    bodyClass: {},
    formClass: {},
    innerFormClass: {},
    headerClass: { default: "p-6" },
    buttonsClass: {},
    headingClass: {},
    subHeadingClass: {},
    submitLabel: { default: "Submit" },
    allowSubmit: {}
  },
  emits: ["success", "error", "done", "update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const s = e, o = l, r = T(), u = T(1), d = T();
    function f() {
      u.value++, _.value = Z(), Oe()?.proxy?.$forceUpdate();
    }
    async function v(j) {
      Object.assign(_.value, j), f(), await At(() => null);
    }
    rl("ModalProvider", {
      openModal: g
    });
    const L = T(), w = T();
    function g(j, ue) {
      L.value = j, w.value = ue;
    }
    async function h(j) {
      w.value && w.value(j), L.value = void 0, w.value = void 0;
    }
    const b = cl(), { getTypeName: A } = Xs(), { typeOf: B, Crud: le, createDto: E } = lt(), I = T(new We()), K = c(() => s.panelClass || Be.panelClass(s.formStyle)), U = c(() => s.formClass || s.formStyle == "card" ? "shadow sm:rounded-md" : qt.formClass), S = c(() => s.headingClass || Be.headingClass(s.formStyle)), V = c(() => s.subHeadingClass || Be.subHeadingClass(s.formStyle)), ee = c(() => typeof s.buttonsClass == "string" ? s.buttonsClass : Be.buttonsClass), re = c(() => s.type ? A(s.type) : s.modelValue?.getTypeName ? s.modelValue.getTypeName() : null), O = c(() => s.metaType ?? B(re.value)), Z = () => s.modelValue || J(), _ = T(Z()), ne = c(() => b.loading.value), F = c(() => s.heading != null ? s.heading : O.value?.description || Pe(re.value));
    t({ forceUpdate: f, props: s, setModel: v, formFields: r, submit: P, close: oe, model: _ });
    function J() {
      return typeof s.type == "string" ? E(s.type) : s.type ? new s.type() : s.modelValue;
    }
    async function C(j) {
      if (!j || j.tagName != "FORM") {
        console.error("Not a valid form", j);
        return;
      }
      const ue = J();
      let fe = Ne(ue?.getMethod, (pe) => typeof pe == "function" ? pe() : null) || "POST", se = Ne(ue?.createResponse, (pe) => typeof pe == "function" ? pe() : null) == null;
      const ve = s.jsconfig;
      if (fn.hasRequestBody(fe)) {
        let pe = new ue.constructor(), de = new FormData(j);
        se ? I.value = await b.apiFormVoid(pe, de, { jsconfig: ve }) : I.value = await b.apiForm(pe, de, { jsconfig: ve });
      } else {
        let pe = new ue.constructor(Ia(_.value));
        console.debug("AutoForm.submit", pe), se ? I.value = await b.apiVoid(pe, { jsconfig: ve }) : I.value = await b.api(pe, { jsconfig: ve });
      }
      I.value.succeeded ? (o("success", I.value.response), oe()) : o("error", I.value.error);
    }
    async function P() {
      C(d.value);
    }
    function Q(j) {
      o("update:modelValue", j);
    }
    function k() {
      o("done");
    }
    const p = T(!1), te = T(""), Y = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    xt(p, () => {
      wt(Y, te, p.value), p.value || setTimeout(k, 700);
    }), p.value = !0;
    function oe() {
      s.formStyle == "slideOver" ? p.value = !1 : k();
    }
    const N = (j) => {
      j.key === "Escape" && oe();
    };
    return Ye(() => window.addEventListener("keydown", N)), jt(() => window.removeEventListener("keydown", N)), (j, ue) => {
      const fe = z("AutoFormFields"), se = z("FormLoading"), ve = z("PrimaryButton"), pe = z("CloseButton"), de = z("SecondaryButton"), ce = z("ModalLookup");
      return a(), i("div", null, [
        O.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: m(K.value)
        }, [
          n("form", {
            ref_key: "elForm",
            ref: d,
            onSubmit: ue[0] || (ue[0] = He((Le) => C(Le.target), ["prevent"])),
            autocomplete: "off",
            class: m(e.innerFormClass)
          }, [
            n("div", {
              class: m(e.bodyClass)
            }, [
              n("div", {
                class: m(e.headerClass)
              }, [
                j.$slots.heading ? (a(), i("div", lc, [
                  H(j.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: m(S.value)
                }, M(F.value), 3)),
                j.$slots.subheading ? (a(), i("div", nc, [
                  H(j.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: m(V.value)
                }, M(e.subHeading), 3)) : O.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: m(["notes", V.value]),
                  innerHTML: O.value?.notes
                }, null, 10, sc)) : x("", !0)
              ], 2),
              H(j.$slots, "header", {
                instance: Oe()?.exposed,
                model: _.value
              }),
              ue[5] || (ue[5] = n("input", {
                type: "submit",
                class: "hidden"
              }, null, -1)),
              (a(), X(fe, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                type: e.type,
                modelValue: _.value,
                "onUpdate:modelValue": Q,
                api: I.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
              H(j.$slots, "footer", {
                instance: Oe()?.exposed,
                model: _.value
              })
            ], 2),
            H(j.$slots, "buttons", {}, () => [
              n("div", {
                class: m(ee.value)
              }, [
                n("div", null, [
                  H(j.$slots, "leftbuttons", {
                    instance: Oe()?.exposed,
                    model: _.value
                  })
                ]),
                n("div", null, [
                  e.showLoading && ne.value ? (a(), X(se, { key: 0 })) : x("", !0)
                ]),
                n("div", ac, [
                  ue[6] || (ue[6] = n("div", null, null, -1)),
                  he(ve, {
                    disabled: ne.value || (e.allowSubmit ? !e.allowSubmit(_.value) : !1)
                  }, {
                    default: xe(() => [
                      we(M(e.submitLabel), 1)
                    ]),
                    _: 1
                  }, 8, ["disabled"]),
                  H(j.$slots, "rightbuttons", {
                    instance: Oe()?.exposed,
                    model: _.value
                  })
                ])
              ], 2)
            ])
          ], 34)
        ], 2)) : (a(), i("div", oc, [
          ue[8] || (ue[8] = n("div", { class: "fixed inset-0" }, null, -1)),
          n("div", ic, [
            n("div", {
              onMousedown: oe,
              class: "absolute inset-0 overflow-hidden"
            }, [
              n("div", {
                onMousedown: ue[2] || (ue[2] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                n("div", {
                  class: m(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", te.value])
                }, [
                  n("form", {
                    ref_key: "elForm",
                    ref: d,
                    class: m(U.value),
                    onSubmit: ue[1] || (ue[1] = He((Le) => C(Le.target), ["prevent"]))
                  }, [
                    n("div", rc, [
                      n("div", uc, [
                        n("div", dc, [
                          n("div", cc, [
                            n("div", fc, [
                              j.$slots.heading ? (a(), i("div", vc, [
                                H(j.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: m(S.value)
                              }, M(F.value), 3)),
                              j.$slots.subheading ? (a(), i("div", mc, [
                                H(j.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: m(V.value)
                              }, M(e.subHeading), 3)) : O.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: m(["notes", V.value]),
                                innerHTML: O.value?.notes
                              }, null, 10, gc)) : x("", !0)
                            ]),
                            n("div", pc, [
                              he(pe, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: oe
                              })
                            ])
                          ])
                        ]),
                        H(j.$slots, "header", {
                          instance: Oe()?.exposed,
                          model: _.value
                        }),
                        (a(), X(fe, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          type: e.type,
                          modelValue: _.value,
                          "onUpdate:modelValue": Q,
                          api: I.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
                        H(j.$slots, "footer", {
                          instance: Oe()?.exposed,
                          model: _.value
                        })
                      ])
                    ]),
                    n("div", {
                      class: m(ee.value)
                    }, [
                      n("div", null, [
                        H(j.$slots, "leftbuttons", {
                          instance: Oe()?.exposed,
                          model: _.value
                        })
                      ]),
                      n("div", null, [
                        e.showLoading && ne.value ? (a(), X(se, { key: 0 })) : x("", !0)
                      ]),
                      n("div", yc, [
                        he(de, {
                          onClick: oe,
                          disabled: ne.value
                        }, {
                          default: xe(() => [...ue[7] || (ue[7] = [
                            we("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"]),
                        he(ve, {
                          class: "ml-4",
                          disabled: ne.value || (e.allowSubmit ? !e.allowSubmit(_.value) : !1)
                        }, {
                          default: xe(() => [
                            we(M(e.submitLabel), 1)
                          ]),
                          _: 1
                        }, 8, ["disabled"]),
                        H(j.$slots, "rightbuttons", {
                          instance: Oe()?.exposed,
                          model: _.value
                        })
                      ])
                    ], 2)
                  ], 34)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), i("div", ec, [
          n("p", tc, [
            ue[3] || (ue[3] = we("Could not create form for unknown ", -1)),
            ue[4] || (ue[4] = n("b", null, "type", -1)),
            we(" " + M(re.value), 1)
          ])
        ])),
        L.value?.name == "ModalLookup" && L.value.ref ? (a(), X(ce, {
          key: 3,
          "ref-info": L.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ]);
    };
  }
}), bc = { key: 0 }, wc = { class: "text-red-700" }, xc = { key: 0 }, kc = { key: 2 }, Cc = ["innerHTML"], $c = { class: "flex justify-end" }, Lc = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, Vc = { class: "fixed inset-0 overflow-hidden" }, Sc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, Mc = { class: "flex-1" }, Tc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, Fc = { class: "flex items-start justify-between space-x-3" }, Ac = { class: "space-y-1" }, Ic = { key: 0 }, Dc = { key: 2 }, Oc = ["innerHTML"], Pc = { class: "flex h-7 items-center" }, jc = { class: "flex justify-end" }, Bc = /* @__PURE__ */ ie({
  __name: "AutoCreateForm",
  props: {
    type: {},
    formStyle: { default: "slideOver" },
    panelClass: {},
    formClass: {},
    headingClass: {},
    subHeadingClass: {},
    buttonsClass: {},
    heading: {},
    subHeading: {},
    autosave: { type: Boolean, default: !0 },
    showLoading: { type: Boolean, default: !0 },
    showCancel: { type: Boolean, default: !0 },
    configureField: {},
    configureFormLayout: {}
  },
  emits: ["done", "save", "error"],
  setup(e, { expose: t, emit: l }) {
    const s = e, o = l, r = T(), u = T(1);
    function d() {
      u.value++, r.value?.forceUpdate(), Oe()?.proxy?.$forceUpdate();
    }
    function f(N) {
      Object.assign(S.value, N), d();
    }
    function v(N) {
    }
    rl("ModalProvider", {
      openModal: g
    });
    const L = T(), w = T();
    function g(N, j) {
      L.value = N, w.value = j;
    }
    async function h(N) {
      w.value && w.value(N), L.value = void 0, w.value = void 0;
    }
    const { typeOf: b, typeProperties: A, Crud: B, createDto: le, formValues: E } = lt(), I = c(() => Ht(s.type)), K = c(() => b(I.value)), S = T(typeof s.type == "string" ? le(s.type) : s.type ? new s.type() : null);
    t({ forceUpdate: d, props: s, setModel: f, formFields: r, model: S });
    const V = c(() => s.panelClass || Be.panelClass(s.formStyle)), ee = c(() => s.formClass || Be.formClass(s.formStyle)), re = c(() => s.headingClass || Be.headingClass(s.formStyle)), O = c(() => s.subHeadingClass || Be.subHeadingClass(s.formStyle)), Z = c(() => s.buttonsClass || Be.buttonsClass), _ = c(() => B.model(K.value)), ne = c(() => s.heading || b(I.value)?.description || (_.value ? `New ${Pe(_.value)}` : Pe(I.value))), F = T(new We());
    let J = cl(), C = c(() => J.loading.value);
    q.interceptors.has("AutoCreateForm.new") && q.interceptors.invoke("AutoCreateForm.new", { props: s, model: S });
    async function P(N) {
      let j = N.target;
      if (!s.autosave) {
        o("save", new S.value.constructor(E(j, A(K.value))));
        return;
      }
      let ue = Ne(S.value?.getMethod, (se) => typeof se == "function" ? se() : null) || "POST", fe = Ne(S.value?.createResponse, (se) => typeof se == "function" ? se() : null) == null;
      if (fn.hasRequestBody(ue)) {
        let se = new S.value.constructor(), ve = new FormData(j);
        fe ? F.value = await J.apiFormVoid(se, ve, { jsconfig: "eccn" }) : F.value = await J.apiForm(se, ve, { jsconfig: "eccn" });
      } else {
        let se = E(j, A(K.value)), ve = new S.value.constructor(se);
        fe ? F.value = await J.apiVoid(ve, { jsconfig: "eccn" }) : F.value = await J.api(ve, { jsconfig: "eccn" });
      }
      F.value.succeeded ? (j.reset(), o("save", F.value.response)) : o("error", F.value.error);
    }
    function Q() {
      o("done");
    }
    const k = T(!1), p = T(""), te = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    xt(k, () => {
      wt(te, p, k.value), k.value || setTimeout(Q, 700);
    }), k.value = !0;
    function Y() {
      s.formStyle == "slideOver" ? k.value = !1 : Q();
    }
    const oe = (N) => {
      N.key === "Escape" && Y();
    };
    return Ye(() => window.addEventListener("keydown", oe)), jt(() => window.removeEventListener("keydown", oe)), (N, j) => {
      const ue = z("AutoFormFields"), fe = z("FormLoading"), se = z("SecondaryButton"), ve = z("PrimaryButton"), pe = z("CloseButton"), de = z("ModalLookup");
      return a(), i("div", null, [
        K.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: m(V.value)
        }, [
          n("form", {
            onSubmit: He(P, ["prevent"])
          }, [
            n("div", {
              class: m(ee.value)
            }, [
              n("div", null, [
                N.$slots.heading ? (a(), i("div", xc, [
                  H(N.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: m(re.value)
                }, M(ne.value), 3)),
                N.$slots.subheading ? (a(), i("div", kc, [
                  H(N.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: m(O.value)
                }, M(e.subHeading), 3)) : K.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: m(["notes", O.value]),
                  innerHTML: K.value?.notes
                }, null, 10, Cc)) : x("", !0)
              ]),
              H(N.$slots, "header", {
                formInstance: Oe()?.exposed,
                model: S.value
              }),
              (a(), X(ue, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                modelValue: S.value,
                "onUpdate:modelValue": v,
                api: F.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              H(N.$slots, "footer", {
                formInstance: Oe()?.exposed,
                model: S.value
              })
            ], 2),
            n("div", {
              class: m(Z.value)
            }, [
              n("div", null, [
                e.showLoading && W(C) ? (a(), X(fe, { key: 0 })) : x("", !0)
              ]),
              n("div", $c, [
                e.showCancel ? (a(), X(se, {
                  key: 0,
                  onClick: Y,
                  disabled: W(C)
                }, {
                  default: xe(() => [...j[3] || (j[3] = [
                    we("Cancel", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])) : x("", !0),
                he(ve, {
                  type: "submit",
                  class: "ml-4",
                  disabled: W(C)
                }, {
                  default: xe(() => [...j[4] || (j[4] = [
                    we("Save", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 32)
        ], 2)) : (a(), i("div", Lc, [
          j[7] || (j[7] = n("div", { class: "fixed inset-0" }, null, -1)),
          n("div", Vc, [
            n("div", {
              onMousedown: Y,
              class: "absolute inset-0 overflow-hidden"
            }, [
              n("div", {
                onMousedown: j[0] || (j[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                n("div", {
                  class: m(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", p.value])
                }, [
                  n("form", {
                    class: m(ee.value),
                    onSubmit: He(P, ["prevent"])
                  }, [
                    n("div", Sc, [
                      n("div", Mc, [
                        n("div", Tc, [
                          n("div", Fc, [
                            n("div", Ac, [
                              N.$slots.heading ? (a(), i("div", Ic, [
                                H(N.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: m(re.value)
                              }, M(ne.value), 3)),
                              N.$slots.subheading ? (a(), i("div", Dc, [
                                H(N.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: m(O.value)
                              }, M(e.subHeading), 3)) : K.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: m(["notes", O.value]),
                                innerHTML: K.value?.notes
                              }, null, 10, Oc)) : x("", !0)
                            ]),
                            n("div", Pc, [
                              he(pe, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: Y
                              })
                            ])
                          ])
                        ]),
                        H(N.$slots, "header", {
                          formInstance: Oe()?.exposed,
                          model: S.value
                        }),
                        (a(), X(ue, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          modelValue: S.value,
                          "onUpdate:modelValue": v,
                          api: F.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
                        H(N.$slots, "footer", {
                          formInstance: Oe()?.exposed,
                          model: S.value
                        })
                      ])
                    ]),
                    n("div", {
                      class: m(Z.value)
                    }, [
                      n("div", null, [
                        e.showLoading && W(C) ? (a(), X(fe, { key: 0 })) : x("", !0)
                      ]),
                      n("div", jc, [
                        e.showCancel ? (a(), X(se, {
                          key: 0,
                          onClick: Y,
                          disabled: W(C)
                        }, {
                          default: xe(() => [...j[5] || (j[5] = [
                            we("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])) : x("", !0),
                        he(ve, {
                          type: "submit",
                          class: "ml-4",
                          disabled: W(C)
                        }, {
                          default: xe(() => [...j[6] || (j[6] = [
                            we("Save", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])
                      ])
                    ], 2)
                  ], 34)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), i("div", bc, [
          n("p", wc, [
            j[1] || (j[1] = we("Could not create form for unknown ", -1)),
            j[2] || (j[2] = n("b", null, "type", -1)),
            we(" " + M(I.value), 1)
          ])
        ])),
        L.value?.name == "ModalLookup" && L.value.ref ? (a(), X(de, {
          key: 3,
          "ref-info": L.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ]);
    };
  }
}), Hc = { key: 0 }, Rc = { class: "text-red-700" }, Ec = { key: 0 }, Nc = { key: 2 }, zc = ["innerHTML"], Uc = { class: "flex justify-end" }, qc = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, _c = { class: "fixed inset-0 overflow-hidden" }, Qc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, Kc = { class: "flex-1" }, Zc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, Wc = { class: "flex items-start justify-between space-x-3" }, Gc = { class: "space-y-1" }, Jc = { key: 0 }, Xc = { key: 2 }, Yc = ["innerHTML"], e0 = { class: "flex h-7 items-center" }, t0 = { class: "flex justify-end" }, l0 = /* @__PURE__ */ ie({
  __name: "AutoEditForm",
  props: {
    modelValue: {},
    deleteType: {},
    type: {},
    formStyle: { default: "slideOver" },
    panelClass: {},
    formClass: {},
    headingClass: {},
    subHeadingClass: {},
    buttonsClass: {},
    heading: {},
    subHeading: {},
    autosave: { type: Boolean, default: !0 },
    showLoading: { type: Boolean, default: !0 },
    showCancel: { type: Boolean },
    configureField: {},
    configureFormLayout: {}
  },
  emits: ["done", "save", "delete", "error"],
  setup(e, { expose: t, emit: l }) {
    const s = e, o = l, r = T(), u = T(1);
    function d() {
      u.value++, re.value = ee(), Oe()?.proxy?.$forceUpdate();
    }
    function f(de) {
      Object.assign(re.value, de);
    }
    function v(de) {
    }
    rl("ModalProvider", {
      openModal: g
    });
    const L = T(), w = T();
    function g(de, ce) {
      L.value = de, w.value = ce;
    }
    async function h(de) {
      w.value && w.value(de), L.value = void 0, w.value = void 0;
    }
    const { typeOf: b, apiOf: A, typeProperties: B, createFormLayout: le, getPrimaryKey: E, Crud: I, createDto: K, formValues: U } = lt(), S = c(() => Ht(s.type)), V = c(() => b(S.value)), ee = () => typeof s.type == "string" ? K(s.type, ll(s.modelValue)) : s.type ? new s.type(ll(s.modelValue)) : null, re = T(ee());
    t({ forceUpdate: d, props: s, setModel: f, formFields: r, model: re });
    const O = c(() => s.panelClass || Be.panelClass(s.formStyle)), Z = c(() => s.formClass || Be.formClass(s.formStyle)), _ = c(() => s.headingClass || Be.headingClass(s.formStyle)), ne = c(() => s.subHeadingClass || Be.subHeadingClass(s.formStyle)), F = c(() => s.buttonsClass || Be.buttonsClass), J = c(() => I.model(V.value)), C = c(() => s.heading || b(S.value)?.description || (J.value ? `Update ${Pe(J.value)}` : Pe(S.value))), P = T(new We());
    let Q = Object.assign({}, ll(s.modelValue));
    q.interceptors.has("AutoEditForm.new") && q.interceptors.invoke("AutoEditForm.new", { props: s, model: re, origModel: Q });
    let k = cl(), p = c(() => k.loading.value);
    const te = () => Ne(b(I.model(V.value)), (de) => E(de));
    function Y(de) {
      const { op: ce, prop: Le } = de;
      ce && (I.isPatch(ce) || I.isUpdate(ce)) && (de.disabled = Le?.isPrimaryKey), s.configureField && s.configureField(de);
    }
    async function oe(de) {
      let ce = de.target;
      if (!s.autosave) {
        o("save", new re.value.constructor(U(ce, B(V.value))));
        return;
      }
      let Le = Ne(re.value?.getMethod, (Me) => typeof Me == "function" ? Me() : null) || "POST", Ie = Ne(re.value?.createResponse, (Me) => typeof Me == "function" ? Me() : null) == null, Ce = te();
      if (fn.hasRequestBody(Le)) {
        let Me = new re.value.constructor(), Te = be(s.modelValue, Ce.name), qe = new FormData(ce);
        Ce && !Array.from(qe.keys()).some((ut) => ut.toLowerCase() == Ce.name.toLowerCase()) && qe.append(Ce.name, Te);
        let nt = [];
        const et = S.value && A(S.value);
        if (et && I.isPatch(et)) {
          let ut = le(V.value), vt = {};
          if (Ce && (vt[Ce.name] = Te), ut.forEach((Re) => {
            let st = Re.id, yt = be(Q, st);
            if (Ce && Ce.name.toLowerCase() === st.toLowerCase())
              return;
            let G = qe.get(st);
            q.interceptors.has("AutoEditForm.save.formLayout") && q.interceptors.invoke("AutoEditForm.save.formLayout", { origValue: yt, formLayout: ut, input: Re, newValue: G });
            let R = G != null, Fe = Re.type === "checkbox" ? R !== !!yt : Re.type === "file" ? R : G != yt;
            !G && !yt && (Fe = !1), Fe && (G ? vt[st] = G : Re.type !== "file" && nt.push(st));
          }), q.interceptors.has("AutoEditForm.save") && q.interceptors.invoke("AutoEditForm.save", { origModel: Q, formLayout: ut, dirtyValues: vt }), Array.from(qe.keys()).filter((Re) => !vt[Re]).forEach((Re) => qe.delete(Re)), Array.from(qe.keys()).filter((Re) => Re.toLowerCase() != Ce.name.toLowerCase()).length == 0 && nt.length == 0) {
            ve();
            return;
          }
        }
        const rt = nt.length > 0 ? { jsconfig: "eccn", reset: nt } : { jsconfig: "eccn" };
        Ie ? P.value = await k.apiFormVoid(Me, qe, rt) : P.value = await k.apiForm(Me, qe, rt);
      } else {
        let Me = U(ce, B(V.value));
        Ce && !be(Me, Ce.name) && (Me[Ce.name] = be(s.modelValue, Ce.name));
        let Te = new re.value.constructor(Me);
        Ie ? P.value = await k.apiVoid(Te, { jsconfig: "eccn" }) : P.value = await k.api(Te, { jsconfig: "eccn" });
      }
      P.value.succeeded ? (ce.reset(), o("save", P.value.response)) : o("error", P.value.error);
    }
    async function N(de) {
      let ce = te();
      const Le = ce ? be(s.modelValue, ce.name) : null;
      if (!Le) {
        console.error(`Could not find Primary Key for Type ${S.value} (${J.value})`);
        return;
      }
      const Ie = { [ce.name]: Le }, Ce = typeof s.deleteType == "string" ? K(s.deleteType, Ie) : s.deleteType ? new s.deleteType(Ie) : null;
      Ne(Ce.createResponse, (Te) => typeof Te == "function" ? Te() : null) == null ? P.value = await k.apiVoid(Ce) : P.value = await k.api(Ce), P.value.succeeded ? o("delete", P.value.response) : o("error", P.value.error);
    }
    function j() {
      o("done");
    }
    const ue = T(!1), fe = T(""), se = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    xt(ue, () => {
      wt(se, fe, ue.value), ue.value || setTimeout(j, 700);
    }), ue.value = !0;
    function ve() {
      s.formStyle == "slideOver" ? ue.value = !1 : j();
    }
    const pe = (de) => {
      de.key === "Escape" && ve();
    };
    return Ye(() => window.addEventListener("keydown", pe)), jt(() => window.removeEventListener("keydown", pe)), (de, ce) => {
      const Le = z("AutoFormFields"), Ie = z("ConfirmDelete"), Ce = z("FormLoading"), Me = z("SecondaryButton"), Te = z("PrimaryButton"), qe = z("CloseButton"), nt = z("ModalLookup");
      return a(), i("div", null, [
        V.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: m(O.value)
        }, [
          n("form", {
            onSubmit: He(oe, ["prevent"])
          }, [
            n("div", {
              class: m(Z.value)
            }, [
              n("div", null, [
                de.$slots.heading ? (a(), i("div", Ec, [
                  H(de.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: m(_.value)
                }, M(C.value), 3)),
                de.$slots.subheading ? (a(), i("div", Nc, [
                  H(de.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: m(ne.value)
                }, M(e.subHeading), 3)) : V.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: m(["notes", ne.value]),
                  innerHTML: V.value?.notes
                }, null, 10, zc)) : x("", !0)
              ]),
              H(de.$slots, "header", {
                formInstance: Oe()?.exposed,
                model: re.value
              }),
              (a(), X(Le, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                modelValue: re.value,
                "onUpdate:modelValue": v,
                api: P.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              H(de.$slots, "footer", {
                formInstance: Oe()?.exposed,
                model: re.value
              })
            ], 2),
            n("div", {
              class: m(F.value)
            }, [
              n("div", null, [
                e.deleteType ? (a(), X(Ie, {
                  key: 0,
                  onDelete: N
                })) : x("", !0)
              ]),
              n("div", null, [
                e.showLoading && W(p) ? (a(), X(Ce, { key: 0 })) : x("", !0)
              ]),
              n("div", Uc, [
                e.showCancel ? (a(), X(Me, {
                  key: 0,
                  onClick: ve,
                  disabled: W(p)
                }, {
                  default: xe(() => [...ce[3] || (ce[3] = [
                    we("Cancel", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])) : x("", !0),
                he(Te, {
                  type: "submit",
                  class: "ml-4",
                  disabled: W(p)
                }, {
                  default: xe(() => [...ce[4] || (ce[4] = [
                    we("Save", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 32)
        ], 2)) : (a(), i("div", qc, [
          ce[7] || (ce[7] = n("div", { class: "fixed inset-0" }, null, -1)),
          n("div", _c, [
            n("div", {
              onMousedown: ve,
              class: "absolute inset-0 overflow-hidden"
            }, [
              n("div", {
                onMousedown: ce[0] || (ce[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                n("div", {
                  class: m(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", fe.value])
                }, [
                  n("form", {
                    class: m(Z.value),
                    onSubmit: He(oe, ["prevent"])
                  }, [
                    n("div", Qc, [
                      n("div", Kc, [
                        n("div", Zc, [
                          n("div", Wc, [
                            n("div", Gc, [
                              de.$slots.heading ? (a(), i("div", Jc, [
                                H(de.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: m(_.value)
                              }, M(C.value), 3)),
                              de.$slots.subheading ? (a(), i("div", Xc, [
                                H(de.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: m(ne.value)
                              }, M(e.subHeading), 3)) : V.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: m(["notes", ne.value]),
                                innerHTML: V.value?.notes
                              }, null, 10, Yc)) : x("", !0)
                            ]),
                            n("div", e0, [
                              he(qe, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: ve
                              })
                            ])
                          ])
                        ]),
                        H(de.$slots, "header", {
                          formInstance: Oe()?.exposed,
                          model: re.value
                        }),
                        (a(), X(Le, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          modelValue: re.value,
                          "onUpdate:modelValue": v,
                          api: P.value,
                          configureField: Y,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureFormLayout"])),
                        H(de.$slots, "footer", {
                          formInstance: Oe()?.exposed,
                          model: re.value
                        })
                      ])
                    ]),
                    n("div", {
                      class: m(F.value)
                    }, [
                      n("div", null, [
                        e.deleteType ? (a(), X(Ie, {
                          key: 0,
                          onDelete: N
                        })) : x("", !0)
                      ]),
                      n("div", null, [
                        e.showLoading && W(p) ? (a(), X(Ce, { key: 0 })) : x("", !0)
                      ]),
                      n("div", t0, [
                        e.showCancel ? (a(), X(Me, {
                          key: 0,
                          onClick: ve,
                          disabled: W(p)
                        }, {
                          default: xe(() => [...ce[5] || (ce[5] = [
                            we("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])) : x("", !0),
                        he(Te, {
                          type: "submit",
                          class: "ml-4",
                          disabled: W(p)
                        }, {
                          default: xe(() => [...ce[6] || (ce[6] = [
                            we("Save", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])
                      ])
                    ], 2)
                  ], 34)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), i("div", Hc, [
          n("p", Rc, [
            ce[1] || (ce[1] = we("Could not create form for unknown ", -1)),
            ce[2] || (ce[2] = n("b", null, "type", -1)),
            we(" " + M(S.value), 1)
          ])
        ])),
        L.value?.name == "ModalLookup" && L.value.ref ? (a(), X(nt, {
          key: 3,
          "ref-info": L.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ]);
    };
  }
}), n0 = { key: 0 }, s0 = { class: "text-red-700" }, a0 = { key: 0 }, o0 = { key: 2 }, i0 = ["innerHTML"], r0 = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, u0 = { class: "fixed inset-0 overflow-hidden" }, d0 = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, c0 = { class: "flex-1" }, f0 = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, v0 = { class: "flex items-start justify-between space-x-3" }, m0 = { class: "space-y-1" }, g0 = { key: 0 }, p0 = { key: 2 }, y0 = ["innerHTML"], h0 = { class: "flex h-7 items-center" }, b0 = /* @__PURE__ */ ie({
  __name: "AutoViewForm",
  props: {
    model: {},
    apis: {},
    typeName: {},
    done: {},
    formStyle: { default: "slideOver" },
    panelClass: {},
    formClass: {},
    headingClass: {},
    subHeadingClass: {},
    heading: {},
    subHeading: {},
    showLoading: { type: Boolean },
    deleteType: {}
  },
  emits: ["done", "save", "delete", "error"],
  setup(e, { emit: t }) {
    const l = e, s = t, { typeOf: o, getPrimaryKey: r, Crud: u, createDto: d } = lt(), f = c(() => l.typeName ?? l.apis.dataModel.name), v = c(() => o(f.value)), y = c(() => l.panelClass || Be.panelClass(l.formStyle)), L = c(() => l.formClass || Be.formClass(l.formStyle)), w = c(() => l.headingClass || Be.headingClass(l.formStyle)), g = c(() => l.subHeadingClass || Be.subHeadingClass(l.formStyle)), h = c(() => l.heading || o(f.value)?.description || (l.model?.id ? `${Pe(f.value)} ${l.model.id}` : "View " + Pe(f.value))), b = T(new We());
    Object.assign({}, ll(l.model)), q.interceptors.has("AutoViewForm.new") && q.interceptors.invoke("AutoViewForm.new", { props: l });
    let A = cl(), B = c(() => A.loading.value);
    const le = () => Ne(v.value, (O) => r(O)), E = c(() => v.value);
    async function I(O) {
      let Z = le();
      const _ = Z ? be(l.model, Z.name) : null;
      if (!_) {
        console.error(`Could not find Primary Key for Type ${f.value} (${E.value})`);
        return;
      }
      const ne = { [Z.name]: _ }, F = typeof l.deleteType == "string" ? d(l.deleteType, ne) : l.deleteType ? new l.deleteType(ne) : null;
      Ne(F.createResponse, (C) => typeof C == "function" ? C() : null) == null ? b.value = await A.apiVoid(F) : b.value = await A.api(F), b.value.succeeded ? s("delete", b.value.response) : s("error", b.value.error);
    }
    function K() {
      l.done && l.done();
    }
    const U = T(!1), S = T(""), V = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    xt(U, () => {
      wt(V, S, U.value), U.value || setTimeout(K, 700);
    }), U.value = !0;
    function ee() {
      l.formStyle == "slideOver" ? U.value = !1 : K();
    }
    const re = (O) => {
      O.key === "Escape" && ee();
    };
    return Ye(() => window.addEventListener("keydown", re)), jt(() => window.removeEventListener("keydown", re)), (O, Z) => {
      const _ = z("MarkupModel"), ne = z("CloseButton"), F = z("ConfirmDelete"), J = z("FormLoading");
      return a(), i("div", null, [
        f.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: m(y.value)
        }, [
          n("div", {
            class: m(L.value)
          }, [
            n("div", null, [
              O.$slots.heading ? (a(), i("div", a0, [
                H(O.$slots, "heading")
              ])) : (a(), i("h3", {
                key: 1,
                class: m(w.value)
              }, M(h.value), 3)),
              O.$slots.subheading ? (a(), i("div", o0, [
                H(O.$slots, "subheading")
              ])) : e.subHeading ? (a(), i("p", {
                key: 3,
                class: m(g.value)
              }, M(e.subHeading), 3)) : v.value?.notes ? (a(), i("p", {
                key: 4,
                class: m(["notes", g.value]),
                innerHTML: v.value?.notes
              }, null, 10, i0)) : x("", !0)
            ]),
            he(_, { value: e.model }, null, 8, ["value"])
          ], 2)
        ], 2)) : (a(), i("div", r0, [
          Z[4] || (Z[4] = n("div", { class: "fixed inset-0" }, null, -1)),
          n("div", u0, [
            n("div", {
              onMousedown: ee,
              class: "absolute inset-0 overflow-hidden"
            }, [
              n("div", {
                onMousedown: Z[0] || (Z[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                n("div", {
                  class: m(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", S.value])
                }, [
                  n("div", {
                    class: m(L.value)
                  }, [
                    n("div", d0, [
                      n("div", c0, [
                        n("div", f0, [
                          n("div", v0, [
                            n("div", m0, [
                              O.$slots.heading ? (a(), i("div", g0, [
                                H(O.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: m(w.value)
                              }, M(h.value), 3)),
                              O.$slots.subheading ? (a(), i("div", p0, [
                                H(O.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: m(g.value)
                              }, M(e.subHeading), 3)) : v.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: m(["notes", g.value]),
                                innerHTML: v.value?.notes
                              }, null, 10, y0)) : x("", !0)
                            ]),
                            n("div", h0, [
                              he(ne, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: ee
                              })
                            ])
                          ])
                        ]),
                        he(_, { value: e.model }, null, 8, ["value"])
                      ])
                    ]),
                    n("div", {
                      class: m(W(Be).buttonsClass)
                    }, [
                      n("div", null, [
                        e.deleteType ? (a(), X(F, {
                          key: 0,
                          onDelete: I
                        })) : x("", !0)
                      ]),
                      n("div", null, [
                        e.showLoading && W(B) ? (a(), X(J, { key: 0 })) : x("", !0)
                      ]),
                      Z[3] || (Z[3] = n("div", { class: "flex justify-end" }, null, -1))
                    ], 2)
                  ], 2)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), i("div", n0, [
          n("p", s0, [
            Z[1] || (Z[1] = we("Could not create view for unknown ", -1)),
            Z[2] || (Z[2] = n("b", null, "type", -1)),
            we(" " + M(f.value), 1)
          ])
        ]))
      ]);
    };
  }
}), w0 = /* @__PURE__ */ ie({
  __name: "ConfirmDelete",
  emits: ["delete"],
  setup(e, { emit: t }) {
    let l = T(!1);
    const s = t, o = () => {
      l.value && s("delete");
    }, r = c(() => [
      "select-none inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white",
      l.value ? "cursor-pointer bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500" : "bg-red-400"
    ]);
    return (u, d) => (a(), i($e, null, [
      It(n("input", {
        id: "confirmDelete",
        type: "checkbox",
        class: "focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800 dark:ring-offset-black",
        "onUpdate:modelValue": d[0] || (d[0] = (f) => ss(l) ? l.value = f : l = f)
      }, null, 512), [
        [dn, W(l)]
      ]),
      d[2] || (d[2] = n("label", {
        for: "confirmDelete",
        class: "ml-2 mr-2 select-none"
      }, "confirm", -1)),
      n("span", ke({
        onClick: He(o, ["prevent"]),
        class: r.value
      }, u.$attrs), [
        H(u.$slots, "default", {}, () => [
          d[1] || (d[1] = we("Delete", -1))
        ])
      ], 16)
    ], 64));
  }
}), x0 = {
  class: "flex",
  title: "loading..."
}, k0 = {
  key: 0,
  xmlns: "http://www.w3.org/2000/svg",
  x: "0px",
  y: "0px",
  width: "24px",
  height: "30px",
  viewBox: "0 0 24 30"
}, C0 = { class: "ml-2 mt-1 text-gray-400" }, $0 = /* @__PURE__ */ ie({
  __name: "FormLoading",
  props: {
    icon: { type: Boolean, default: !0 },
    text: { default: "loading..." }
  },
  setup(e) {
    return ze("ApiState", void 0), (t, l) => (a(), i("div", x0, [
      e.icon ? (a(), i("svg", k0, [...l[0] || (l[0] = [
        Sl('<rect x="0" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="8" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="16" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate></rect>', 3)
      ])])) : x("", !0),
      n("span", C0, M(e.text), 1)
    ]));
  }
}), L0 = ["onClick"], V0 = {
  key: 3,
  class: "flex justify-between items-center"
}, S0 = { class: "mr-1 select-none" }, M0 = ["onClick"], T0 = /* @__PURE__ */ ie({
  __name: "DataGrid",
  props: {
    items: { default: () => [] },
    id: { default: "DataGrid" },
    type: {},
    tableStyle: { default: "stripedRows" },
    selectedColumns: {},
    gridClass: {},
    grid2Class: {},
    grid3Class: {},
    grid4Class: {},
    tableClass: {},
    theadClass: {},
    tbodyClass: {},
    theadRowClass: {},
    theadCellClass: {},
    isSelected: {},
    headerTitle: {},
    headerTitles: {},
    visibleFrom: {},
    rowClass: {},
    rowStyle: {}
  },
  emits: ["headerSelected", "rowSelected"],
  setup(e, { emit: t }) {
    const l = e, s = t, o = T(), r = T(null), u = (Q) => r.value === Q, d = Ml(), f = (Q) => Object.keys(d).find((k) => k.toLowerCase() == Q.toLowerCase() + "-header"), v = (Q) => Object.keys(d).find((k) => k.toLowerCase() == Q.toLowerCase()), y = c(() => Xl(l.items).filter((Q) => !!(d[Q] || d[Q + "-header"]))), { typeOf: L, typeProperties: w } = lt(), g = c(() => Ht(l.type)), h = c(() => L(g.value)), b = c(() => w(h.value));
    function A(Q) {
      const k = l.headerTitles && be(l.headerTitles, Q) || Q;
      return l.headerTitle ? l.headerTitle(k) : cs(k);
    }
    function B(Q) {
      const k = Q.toLowerCase();
      return b.value.find((p) => p.name.toLowerCase() == k);
    }
    function le(Q) {
      const k = B(Q);
      return k?.format ? k.format : k?.type == "TimeSpan" || k?.type == "TimeOnly" ? { method: "time" } : null;
    }
    const E = {
      xs: "xs:table-cell",
      sm: "sm:table-cell",
      md: "md:table-cell",
      lg: "lg:table-cell",
      xl: "xl:table-cell",
      "2xl": "2xl:table-cell",
      never: ""
    };
    function I(Q) {
      const k = l.visibleFrom && be(l.visibleFrom, Q);
      return k && Ne(E[k], (p) => `hidden ${p}`);
    }
    const K = c(() => l.gridClass ?? ge.getGridClass(l.tableStyle)), U = c(() => l.grid2Class ?? ge.getGrid2Class(l.tableStyle)), S = c(() => l.grid3Class ?? ge.getGrid3Class(l.tableStyle)), V = c(() => l.grid4Class ?? ge.getGrid4Class(l.tableStyle)), ee = c(() => l.tableClass ?? ge.getTableClass(l.tableStyle)), re = c(() => l.tbodyClass ?? ge.getTbodyClass(l.tbodyClass)), O = c(() => l.theadClass ?? ge.getTheadClass(l.tableStyle)), Z = c(() => l.theadRowClass ?? ge.getTheadRowClass(l.tableStyle)), _ = c(() => l.theadCellClass ?? ge.getTheadCellClass(l.tableStyle));
    function ne(Q, k) {
      return l.rowClass ? l.rowClass(Q, k) : ge.getTableRowClass(l.tableStyle, k, !!(l.isSelected && l.isSelected(Q)), l.isSelected != null);
    }
    function F(Q, k) {
      return l.rowStyle ? l.rowStyle(Q, k) : void 0;
    }
    const J = c(() => {
      const Q = (typeof l.selectedColumns == "string" ? l.selectedColumns.split(",") : l.selectedColumns) || (y.value.length > 0 ? y.value : Xl(l.items)), k = b.value.reduce((p, te) => (p[te.name.toLowerCase()] = te.format, p), {});
      return Q.filter((p) => k[p.toLowerCase()]?.method != "hidden");
    });
    function C(Q, k) {
      s("headerSelected", k, Q);
    }
    function P(Q, k, p) {
      s("rowSelected", p, Q);
    }
    return (Q, k) => {
      const p = z("CellFormat"), te = z("PreviewFormat");
      return e.items.length ? (a(), i("div", {
        key: 0,
        ref_key: "refResults",
        ref: o,
        class: m(K.value)
      }, [
        n("div", {
          class: m(U.value)
        }, [
          n("div", {
            class: m(S.value)
          }, [
            n("div", {
              class: m(V.value)
            }, [
              n("table", {
                class: m(ee.value)
              }, [
                n("thead", {
                  class: m(O.value)
                }, [
                  n("tr", {
                    class: m(Z.value)
                  }, [
                    (a(!0), i($e, null, Se(J.value, (Y) => (a(), i("td", {
                      class: m([I(Y), _.value, u(Y) ? "text-gray-900 dark:text-gray-50" : "text-gray-500 dark:text-gray-400"])
                    }, [
                      n("div", {
                        onClick: (oe) => C(oe, Y)
                      }, [
                        W(d)[Y + "-header"] ? H(Q.$slots, Y + "-header", {
                          key: 0,
                          column: Y
                        }) : f(Y) ? H(Q.$slots, f(Y), {
                          key: 1,
                          column: Y
                        }) : W(d).header ? H(Q.$slots, "header", {
                          key: 2,
                          column: Y,
                          label: A(Y)
                        }) : (a(), i("div", V0, [
                          n("span", S0, M(A(Y)), 1)
                        ]))
                      ], 8, L0)
                    ], 2))), 256))
                  ], 2)
                ], 2),
                n("tbody", {
                  class: m(re.value)
                }, [
                  (a(!0), i($e, null, Se(e.items, (Y, oe) => (a(), i("tr", {
                    class: m(ne(Y, oe)),
                    style: un(F(Y, oe)),
                    onClick: (N) => P(N, oe, Y)
                  }, [
                    (a(!0), i($e, null, Se(J.value, (N) => (a(), i("td", {
                      class: m([I(N), W(ge).tableCellClass])
                    }, [
                      W(d)[N] ? H(Q.$slots, N, ke({
                        key: 0,
                        ref_for: !0
                      }, Y)) : v(N) ? H(Q.$slots, v(N), ke({
                        key: 1,
                        ref_for: !0
                      }, Y)) : B(N) ? (a(), X(p, {
                        key: 2,
                        type: h.value,
                        propType: B(N),
                        modelValue: Y
                      }, null, 8, ["type", "propType", "modelValue"])) : (a(), X(te, {
                        key: 3,
                        value: W(be)(Y, N),
                        format: le(N)
                      }, null, 8, ["value", "format"]))
                    ], 2))), 256))
                  ], 14, M0))), 256))
                ], 2)
              ], 2)
            ], 2)
          ], 2)
        ], 2)
      ], 2)) : x("", !0);
    };
  }
}), F0 = ie({
  props: {
    type: Object,
    propType: Object,
    modelValue: Object
  },
  setup(e, { attrs: t }) {
    const { typeOf: l } = lt();
    function s(o) {
      return o?.format ? o.format : o?.type == "TimeSpan" || o?.type == "TimeOnly" ? { method: "time" } : null;
    }
    return () => {
      const o = s(e.propType), r = be(e.modelValue, e.propType.name), u = Object.assign({}, e, t), d = Vt("span", { innerHTML: ol(r, o, u) }), f = il(r) && Array.isArray(r) ? Vt("span", {}, [
        Vt("span", { class: "mr-2" }, `${r.length}`),
        d
      ]) : d, v = e.propType?.ref;
      if (!v)
        return f;
      const L = Xe(e.type).find((B) => B.type === v.model);
      if (!L)
        return f;
      const w = be(e.modelValue, L.name), g = w && v.refLabel && be(w, v.refLabel);
      if (!g)
        return f;
      const b = l(v.model)?.icon, A = b ? Vt(ta, { image: b, class: "w-5 h-5 mr-1" }) : null;
      return Vt("span", { class: "flex", title: `${v.model} ${r}` }, [
        A,
        g
      ]);
    };
  }
}), A0 = { key: 0 }, I0 = {
  key: 0,
  class: "mr-2"
}, D0 = ["innerHTML"], O0 = ["innerHTML"], P0 = {
  inheritAttrs: !1
}, j0 = /* @__PURE__ */ ie({
  ...P0,
  __name: "PreviewFormat",
  props: {
    value: {},
    format: {},
    includeIcon: { type: Boolean, default: !0 },
    includeCount: { type: Boolean, default: !0 },
    maxFieldLength: { default: 150 },
    maxNestedFields: { default: 2 },
    maxNestedFieldLength: { default: 30 }
  },
  setup(e) {
    const t = e, l = c(() => Array.isArray(t.value));
    return (s, o) => W(il)(e.value) ? (a(), i("span", A0, [
      e.includeCount && l.value ? (a(), i("span", I0, M(e.value.length), 1)) : x("", !0),
      n("span", {
        innerHTML: W(ol)(e.value, e.format, s.$attrs)
      }, null, 8, D0)
    ])) : (a(), i("span", {
      key: 1,
      innerHTML: W(ol)(e.value, e.format, s.$attrs)
    }, null, 8, O0));
  }
}), B0 = ["innerHTML"], H0 = { key: 0 }, R0 = { key: 2 }, E0 = /* @__PURE__ */ ie({
  __name: "HtmlFormat",
  props: {
    value: {},
    depth: { default: 0 },
    fieldAttrs: {},
    classes: { type: Function, default: (e, t, l, s, o) => s }
  },
  setup(e) {
    const t = e, l = c(() => Pt(t.value)), s = c(() => Array.isArray(t.value)), o = (f) => cs(f), r = (f) => t.fieldAttrs ? t.fieldAttrs(f) : null, u = c(() => Xl(t.value)), d = (f) => f ? Object.keys(f).map((v) => ({ key: o(v), val: f[v] })) : [];
    return (f, v) => {
      const y = z("HtmlFormat", !0);
      return a(), i("div", {
        class: m(e.depth == 0 ? "prose html-format" : "")
      }, [
        l.value ? (a(), i("div", {
          key: 0,
          innerHTML: W(ol)(e.value)
        }, null, 8, B0)) : s.value ? (a(), i("div", {
          key: 1,
          class: m(e.classes("array", "div", e.depth, W(ge).gridClass))
        }, [
          W(Pt)(e.value[0]) ? (a(), i("div", H0, "[ " + M(e.value.join(", ")) + " ]", 1)) : (a(), i("div", {
            key: 1,
            class: m(e.classes("array", "div", e.depth, W(ge).grid2Class))
          }, [
            n("div", {
              class: m(e.classes("array", "div", e.depth, W(ge).grid3Class))
            }, [
              n("div", {
                class: m(e.classes("array", "div", e.depth, W(ge).grid4Class))
              }, [
                n("table", {
                  class: m(e.classes("object", "table", e.depth, W(ge).tableClass))
                }, [
                  n("thead", {
                    class: m(e.classes("array", "thead", e.depth, W(ge).theadClass))
                  }, [
                    n("tr", null, [
                      (a(!0), i($e, null, Se(u.value, (L) => (a(), i("th", {
                        class: m(e.classes("array", "th", e.depth, W(ge).theadCellClass + " whitespace-nowrap"))
                      }, [
                        v[0] || (v[0] = n("b", null, null, -1)),
                        we(M(o(L)), 1)
                      ], 2))), 256))
                    ])
                  ], 2),
                  n("tbody", null, [
                    (a(!0), i($e, null, Se(e.value, (L, w) => (a(), i("tr", {
                      class: m(e.classes("array", "tr", e.depth, w % 2 == 0 ? "bg-white" : "bg-gray-50", w))
                    }, [
                      (a(!0), i($e, null, Se(u.value, (g) => (a(), i("td", {
                        class: m(e.classes("array", "td", e.depth, W(ge).tableCellClass))
                      }, [
                        he(y, ke({
                          value: L[g],
                          "field-attrs": e.fieldAttrs,
                          depth: e.depth + 1,
                          classes: e.classes
                        }, { ref_for: !0 }, r(g)), null, 16, ["value", "field-attrs", "depth", "classes"])
                      ], 2))), 256))
                    ], 2))), 256))
                  ])
                ], 2)
              ], 2)
            ], 2)
          ], 2))
        ], 2)) : (a(), i("div", R0, [
          n("table", {
            class: m(e.classes("object", "table", e.depth, "table-object"))
          }, [
            (a(!0), i($e, null, Se(d(e.value), (L) => (a(), i("tr", {
              class: m(e.classes("object", "tr", e.depth, ""))
            }, [
              n("th", {
                class: m(e.classes("object", "th", e.depth, "align-top py-2 px-4 text-left text-sm font-medium tracking-wider whitespace-nowrap"))
              }, M(L.key), 3),
              n("td", {
                class: m(e.classes("object", "td", e.depth, "align-top py-2 px-4 text-sm"))
              }, [
                he(y, ke({
                  value: L.val,
                  "field-attrs": e.fieldAttrs,
                  depth: e.depth + 1,
                  classes: e.classes
                }, { ref_for: !0 }, r(L.key)), null, 16, ["value", "field-attrs", "depth", "classes"])
              ], 2)
            ], 2))), 256))
          ], 2)
        ]))
      ], 2);
    };
  }
}), N0 = ["href"], z0 = ["href", "title"], U0 = /* @__PURE__ */ ie({
  __name: "MarkupFormat",
  props: {
    value: {},
    imageClass: { default: "w-8 h-8" }
  },
  setup(e) {
    const t = e, { getMimeType: l } = Ha(), s = t.value;
    let o = typeof t.value;
    const r = o === "string" && s.length ? l(s) : null;
    if (o === "string" && s.length) {
      const u = s.startsWith("https://") || s.startsWith("http://");
      (u || s[0] === "/") && r?.startsWith("image/") ? o = "image" : u && (o = "link");
    }
    return (u, d) => {
      const f = z("Icon"), v = z("HtmlFormat");
      return W(o) == "link" ? (a(), i("a", {
        key: 0,
        href: e.value,
        class: "text-indigo-600"
      }, M(e.value), 9, N0)) : W(o) == "image" ? (a(), i("a", {
        key: 1,
        href: e.value,
        title: e.value,
        class: "inline-block"
      }, [
        he(f, {
          src: e.value,
          class: m(e.imageClass)
        }, null, 8, ["src", "class"])
      ], 8, z0)) : (a(), X(v, {
        key: 2,
        value: e.value
      }, null, 8, ["value"]));
    };
  }
}), q0 = { class: "my-2 w-full" }, _0 = { class: "leading-7" }, Q0 = { class: "px-2 text-left align-top" }, K0 = { colspan: "align-top" }, Z0 = { class: "my-2 leading-7" }, W0 = {
  colspan: "2",
  class: "px-2 bg-indigo-700 text-white"
}, G0 = { class: "leading-7" }, J0 = {
  colspan: "2",
  class: "px-2 align-top"
}, X0 = /* @__PURE__ */ ie({
  __name: "MarkupModel",
  props: {
    value: {},
    imageClass: {}
  },
  setup(e) {
    const t = e, l = Object.keys(t.value), s = {}, o = {};
    return l.forEach((r) => {
      const u = t.value[r], d = typeof u;
      u == null || d === "function" || d === "symbol" ? s[r] = `(${u == null ? "null" : "t"})` : d === "object" ? o[r] = u : s[r] = u;
    }), (r, u) => {
      const d = z("MarkupFormat");
      return a(), i("table", q0, [
        (a(), i($e, null, Se(s, (f, v) => n("tr", _0, [
          n("th", Q0, M(W(Pe)(v)), 1),
          n("td", K0, [
            he(d, { value: f }, null, 8, ["value"])
          ])
        ])), 64)),
        (a(), i($e, null, Se(o, (f, v) => (a(), i($e, null, [
          n("tr", Z0, [
            n("td", W0, M(W(Pe)(v)), 1)
          ]),
          n("tr", G0, [
            n("td", J0, [
              he(d, { value: f }, null, 8, ["value"])
            ])
          ])
        ], 64))), 64))
      ]);
    };
  }
}), Y0 = { class: "absolute top-0 right-0 pt-4 pr-4" }, ef = ["title"], tf = /* @__PURE__ */ ie({
  __name: "CloseButton",
  props: {
    buttonClass: { default: "bg-white dark:bg-black" },
    title: { default: "Close" }
  },
  emits: ["close"],
  setup(e, { emit: t }) {
    return (l, s) => (a(), i("div", Y0, [
      n("button", {
        type: "button",
        onClick: s[0] || (s[0] = (o) => l.$emit("close")),
        title: e.title,
        class: m([e.buttonClass, "cursor-pointer rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"])
      }, [...s[1] || (s[1] = [
        n("span", { class: "sr-only" }, "Close", -1),
        n("svg", {
          class: "h-6 w-6",
          xmlns: "http://www.w3.org/2000/svg",
          fill: "none",
          viewBox: "0 0 24 24",
          stroke: "currentColor",
          "aria-hidden": "true"
        }, [
          n("path", {
            "stroke-linecap": "round",
            "stroke-linejoin": "round",
            "stroke-width": "2",
            d: "M6 18L18 6M6 6l12 12"
          })
        ], -1)
      ])], 10, ef)
    ]));
  }
}), lf = ["id", "aria-labelledby"], nf = { class: "fixed inset-0 overflow-hidden" }, sf = { class: "flex h-full flex-col bg-white dark:bg-black shadow-xl" }, af = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, of = { class: "flex-1" }, rf = { class: "relative bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, uf = { class: "flex items-start justify-between space-x-3" }, df = { class: "space-y-1" }, cf = { key: 0 }, ff = ["id"], vf = {
  key: 2,
  class: "text-sm text-gray-500"
}, mf = { class: "flex h-7 items-center" }, gf = {
  key: 0,
  class: "flex-shrink-0 border-t border-gray-200 dark:border-gray-700 px-4 py-5 sm:px-6"
}, pf = /* @__PURE__ */ ie({
  __name: "SlideOver",
  props: {
    id: { default: "SlideOver" },
    title: {},
    contentClass: { default: "relative mt-6 flex-1 px-4 sm:px-6" }
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = t, s = T(!1), o = T(""), r = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    xt(s, () => {
      wt(r, o, s.value), s.value || setTimeout(() => l("done"), 700);
    }), s.value = !0;
    const u = () => s.value = !1, d = (f) => {
      f.key === "Escape" && u();
    };
    return Ye(() => window.addEventListener("keydown", d)), jt(() => window.removeEventListener("keydown", d)), (f, v) => {
      const y = z("CloseButton");
      return a(), i("div", {
        id: e.id,
        class: "relative z-10",
        "aria-labelledby": e.id + "-title",
        role: "dialog",
        "aria-modal": "true"
      }, [
        v[1] || (v[1] = n("div", { class: "fixed inset-0" }, null, -1)),
        n("div", nf, [
          n("div", {
            onMousedown: u,
            class: "absolute inset-0 overflow-hidden"
          }, [
            n("div", {
              onMousedown: v[0] || (v[0] = He(() => {
              }, ["stop"])),
              class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
            }, [
              n("div", {
                class: m(["panel pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", o.value])
              }, [
                n("div", sf, [
                  n("div", af, [
                    n("div", of, [
                      n("div", rf, [
                        n("div", uf, [
                          n("div", df, [
                            f.$slots.title ? (a(), i("div", cf, [
                              H(f.$slots, "title")
                            ])) : x("", !0),
                            e.title ? (a(), i("h2", {
                              key: 1,
                              class: "text-lg font-medium text-gray-900 dark:text-gray-50",
                              id: e.id + "-title"
                            }, M(e.title), 9, ff)) : x("", !0),
                            f.$slots.subtitle ? (a(), i("p", vf, [
                              H(f.$slots, "subtitle")
                            ])) : x("", !0)
                          ]),
                          n("div", mf, [
                            he(y, {
                              "button-class": "bg-gray-50 dark:bg-gray-900",
                              onClose: u
                            })
                          ])
                        ])
                      ]),
                      n("div", {
                        class: m(e.contentClass)
                      }, [
                        H(f.$slots, "default")
                      ], 2)
                    ])
                  ]),
                  f.$slots.footer ? (a(), i("div", gf, [
                    H(f.$slots, "footer")
                  ])) : x("", !0)
                ])
              ], 2)
            ], 32)
          ], 32)
        ])
      ], 8, lf);
    };
  }
}), yf = ["id", "data-transition-for", "aria-labelledby"], hf = { class: "fixed inset-0 z-10 overflow-y-auto" }, bf = { class: "flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0" }, wf = {
  key: 1,
  class: "hidden sm:block absolute top-0 right-0 pt-4 pr-4 z-10"
}, xf = /* @__PURE__ */ ie({
  __name: "ModalDialog",
  props: {
    id: { default: "ModalDialog" },
    modalClass: { default: rn.modalClass },
    sizeClass: { default: rn.sizeClass },
    closeButtonClass: { default: "bg-white dark:bg-black cursor-pointer rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black" },
    configureField: {}
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = Ml(), s = t, o = T(!1), r = T(""), u = {
      entering: { cls: "ease-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100", to: "opacity-0" }
    }, d = T(""), f = {
      entering: { cls: "ease-out duration-300", from: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95", to: "opacity-100 translate-y-0 sm:scale-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100 translate-y-0 sm:scale-100", to: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95" }
    };
    xt(o, () => {
      wt(u, r, o.value), wt(f, d, o.value), o.value || setTimeout(() => s("done"), 200);
    }), o.value = !0;
    const v = () => o.value = !1;
    rl("ModalProvider", {
      openModal: g
    });
    const L = T(), w = T();
    function g(A, B) {
      L.value = A, w.value = B;
    }
    async function h(A) {
      w.value && w.value(A), L.value = void 0, w.value = void 0;
    }
    const b = (A) => {
      A.key === "Escape" && v();
    };
    return Ye(() => window.addEventListener("keydown", b)), jt(() => window.removeEventListener("keydown", b)), (A, B) => {
      const le = z("ModalLookup");
      return a(), i("div", {
        id: e.id,
        "data-transition-for": e.id,
        onMousedown: v,
        class: "relative z-10",
        "aria-labelledby": `${e.id}-title`,
        role: "dialog",
        "aria-modal": "true"
      }, [
        n("div", {
          class: m(["fixed inset-0 bg-gray-500/75 transition-opacity", r.value])
        }, null, 2),
        n("div", hf, [
          n("div", bf, [
            n("div", {
              class: m([e.modalClass, e.sizeClass, d.value]),
              onMousedown: B[0] || (B[0] = He(() => {
              }, ["stop"]))
            }, [
              n("div", null, [
                W(l).closebutton ? H(A.$slots, "createform", { key: 0 }) : (a(), i("div", wf, [
                  n("button", {
                    type: "button",
                    onClick: v,
                    class: m(e.closeButtonClass),
                    title: "Close"
                  }, [...B[1] || (B[1] = [
                    n("span", { class: "sr-only" }, "Close", -1),
                    n("svg", {
                      class: "h-6 w-6",
                      xmlns: "http://www.w3.org/2000/svg",
                      fill: "none",
                      viewBox: "0 0 24 24",
                      stroke: "currentColor",
                      "aria-hidden": "true"
                    }, [
                      n("path", {
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round",
                        "stroke-width": "2",
                        d: "M6 18L18 6M6 6l12 12"
                      })
                    ], -1)
                  ])], 2)
                ])),
                H(A.$slots, "default")
              ])
            ], 34),
            H(A.$slots, "bottom")
          ])
        ]),
        L.value?.name == "ModalLookup" && L.value.ref ? (a(), X(le, {
          key: 0,
          "ref-info": L.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ], 40, yf);
    };
  }
}), kf = {
  class: "pt-2 overflow-auto",
  style: { "min-height": "620px" }
}, Cf = { class: "mt-3 pl-5 flex flex-wrap items-center" }, $f = { class: "hidden sm:block text-xl leading-6 font-medium text-gray-900 dark:text-gray-50 mr-3" }, Lf = { class: "hidden md:inline" }, Vf = { class: "flex pb-1 sm:pb-0" }, Sf = ["title"], Mf = ["disabled"], Tf = ["disabled"], Ff = ["disabled"], Af = ["disabled"], If = {
  key: 0,
  class: "flex pb-1 sm:pb-0"
}, Df = { class: "px-4 text-lg text-black dark:text-white" }, Of = { key: 0 }, Pf = { key: 1 }, jf = { key: 2 }, Bf = {
  key: 1,
  class: "pl-2 mt-1"
}, Hf = { class: "whitespace-nowrap" }, Rf = {
  key: 2,
  class: "pl-2"
}, Ef = { class: "flex pb-1 sm:pb-0" }, Nf = {
  key: 0,
  class: "pl-2"
}, zf = { class: "mr-1" }, Uf = {
  key: 0,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, qf = {
  key: 1,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, _f = { key: 1 }, Qf = { key: 4 }, Kf = { key: 0 }, Zf = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, Wf = { class: "mr-1 select-none" }, Gf = {
  key: 1,
  class: "flex justify-between items-center"
}, Jf = { class: "mr-1 select-none" }, ns = 25, Xf = /* @__PURE__ */ ie({
  __name: "ModalLookup",
  props: {
    id: { default: "ModalLookup" },
    refInfo: {},
    skip: { default: 0 },
    prefs: {},
    selectedColumns: {},
    allowFiltering: { type: [Boolean, null], default: !0 },
    showPreferences: { type: [Boolean, null], default: !0 },
    showPagingNav: { type: [Boolean, null], default: !0 },
    showPagingInfo: { type: [Boolean, null], default: !0 },
    showResetPreferences: { type: [Boolean, null], default: !0 },
    showFiltersView: { type: [Boolean, null], default: !0 },
    toolbarButtonClass: {},
    canFilter: {},
    type: {},
    modelTitle: {},
    newButtonLabel: {},
    configureField: {}
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = e, s = t, o = Ml(), { config: r } = Bt(), { metadataApi: u, filterDefinitions: d } = lt(), f = ze("client"), v = r.value.storage, y = c(() => l.toolbarButtonClass ?? ge.toolbarButtonClass), L = c(() => d.value), w = T({ take: ns }), g = T(new We()), h = T(l.skip), b = T(!1), A = T(), B = (G) => typeof G == "string" ? G.split(",") : G || [];
    function le(G, R) {
      return ge.getTableRowClass("fullWidth", R, !1, !0);
    }
    function E() {
      let G = B(l.selectedColumns);
      return G.length > 0 ? G : [];
    }
    const I = c(() => it(l.refInfo.model)), K = c(() => {
      let R = E().map((De) => De.toLowerCase());
      const Fe = Xe(I.value);
      return R.length > 0 ? R.map((De) => Fe.find((_e) => _e.name.toLowerCase() === De)).filter((De) => De != null) : Fe;
    }), U = c(() => {
      let G = K.value.map((Fe) => Fe.name), R = B(w.value.selectedColumns).map((Fe) => Fe.toLowerCase());
      return R.length > 0 ? G.filter((Fe) => R.includes(Fe.toLowerCase())) : G;
    }), S = c(() => w.value.take ?? ns), V = c(() => (g.value.response ? be(g.value.response, "results") : null) ?? []), ee = c(() => g.value.response?.total ?? V.value.length ?? 0), re = c(() => h.value > 0), O = c(() => h.value > 0), Z = c(() => V.value.length >= S.value), _ = c(() => V.value.length >= S.value), ne = T([]), F = c(() => ne.value.some((G) => G.settings.filters.length > 0 || !!G.settings.sort)), J = c(() => ne.value.map((G) => G.settings.filters.length).reduce((G, R) => G + R, 0)), C = c(() => Wt(I.value)), P = c(() => u.value?.operations.find((G) => G.dataModel?.name == l.refInfo.model && Ee.isAnyQuery(G))), Q = T(), k = T(!1), p = T(), te = c(() => Ht(l.refInfo.model)), Y = c(() => Ot.forType(te.value, u.value)), oe = c(() => te.value || P.value?.dataModel.name), N = c(() => l.modelTitle || oe.value), j = c(() => l.newButtonLabel || `New ${N.value}`), ue = c(() => nl(Y.value.Create)), fe = T(), se = T(!1);
    function ve() {
      se.value = !0;
    }
    function pe() {
      se.value = !1;
    }
    async function de(G) {
      pe(), s("done", G);
    }
    function ce(G) {
      fe.value && (Object.assign(fe.value?.model, G), console.log("setCreate", JSON.stringify(G, null, 2)), Le());
    }
    function Le() {
      fe.value?.forceUpdate(), Oe()?.proxy?.$forceUpdate();
    }
    const Ie = () => `${l.id}/ApiPrefs/${l.refInfo.model}`, Ce = (G) => `Column/${l.id}:${l.refInfo.model}.${G}`;
    async function Me(G) {
      h.value += G, h.value < 0 && (h.value = 0);
      var R = Math.floor(ee.value / S.value) * S.value;
      h.value > R && (h.value = R), await Qe();
    }
    async function Te(G, R) {
      s("done", G);
    }
    function qe() {
      s("done", null);
    }
    function nt(G, R) {
      let Fe = R.target;
      if (Fe?.tagName !== "TD") {
        let De = Fe?.closest("TABLE")?.getBoundingClientRect(), _e = ne.value.find((tt) => tt.name.toLowerCase() == G.toLowerCase());
        if (_e && De) {
          let tt = 318, Ct = (R.target?.tagName === "DIV" ? R.target : R.target?.closest("DIV")).getBoundingClientRect(), Ve = tt + 25;
          p.value = {
            column: _e,
            topLeft: {
              x: Math.max(Math.floor(Ct.x + 25), Ve),
              y: Math.floor(115)
            }
          };
        }
      }
    }
    function et() {
      p.value = null;
    }
    async function rt(G) {
      let R = p.value?.column;
      R && (R.settings = G, v.setItem(Ce(R.name), JSON.stringify(R.settings)), await Qe()), p.value = null;
    }
    async function ut(G) {
      v.setItem(Ce(G.name), JSON.stringify(G.settings)), await Qe();
    }
    async function vt(G) {
      k.value = !1, w.value = G, v.setItem(Ie(), JSON.stringify(G)), await Qe();
    }
    async function Qe() {
      await Re(st());
    }
    async function Re(G) {
      const R = P.value;
      if (!R) {
        console.error(`No Query API was found for ${l.refInfo.model}`);
        return;
      }
      let Fe = sl(R, G), De = ds((ht) => {
        g.value.response = g.value.error = void 0, b.value = ht;
      }), _e = await f.api(Fe);
      De(), At(() => g.value = _e);
      let tt = be(_e.response, "results") || [];
      !_e.succeeded || tt.label == 0;
    }
    function st() {
      let G = {
        include: "total",
        take: S.value
      }, R = B(w.value.selectedColumns || l.selectedColumns);
      if (R.length > 0) {
        let De = C.value;
        De && R.includes(De.name) && (R = [De.name, ...R]), G.fields = R.join(",");
      }
      let Fe = [];
      return ne.value.forEach((De) => {
        De.settings.sort && Fe.push((De.settings.sort === "DESC" ? "-" : "") + De.name), De.settings.filters.forEach((_e) => {
          let tt = _e.key.replace("%", De.name);
          G[tt] = _e.value;
        });
      }), typeof G.skip > "u" && h.value > 0 && (G.skip = h.value), Fe.length > 0 && (G.orderBy = Fe.join(",")), G;
    }
    async function yt() {
      ne.value.forEach((G) => {
        G.settings = { filters: [] }, v.removeItem(Ce(G.name));
      }), await Qe();
    }
    return Ye(async () => {
      const G = l.prefs || Ll(v.getItem(Ie()));
      G && (w.value = G), ne.value = K.value.map((R) => ({
        name: R.name,
        type: R.type,
        meta: R,
        settings: Object.assign(
          {
            filters: []
          },
          Ll(v.getItem(Ce(R.name)))
        )
      })), isNaN(l.skip) || (h.value = l.skip), await Qe();
    }), (G, R) => {
      const Fe = z("AutoCreateForm"), De = z("ErrorSummary"), _e = z("Loading"), tt = z("SettingsIcons"), ht = z("DataGrid"), Ct = z("ModalDialog");
      return a(), i($e, null, [
        e.refInfo ? (a(), X(Ct, {
          key: 0,
          ref_key: "modalDialog",
          ref: Q,
          id: e.id,
          onDone: qe
        }, {
          default: xe(() => [
            n("div", kf, [
              n("div", Cf, [
                n("h3", $f, [
                  R[9] || (R[9] = we(" Select ", -1)),
                  n("span", Lf, M(W(Pe)(e.refInfo.model)), 1)
                ]),
                n("div", Vf, [
                  e.showPreferences ? (a(), i("button", {
                    key: 0,
                    type: "button",
                    class: "pl-2 text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                    title: `${e.refInfo.model} Preferences`,
                    onClick: R[0] || (R[0] = (Ve) => k.value = !k.value)
                  }, [...R[10] || (R[10] = [
                    n("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("g", {
                        "stroke-width": "1.5",
                        fill: "none"
                      }, [
                        n("path", {
                          d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
                          stroke: "currentColor"
                        })
                      ])
                    ], -1)
                  ])], 8, Sf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 1,
                    type: "button",
                    class: m(["pl-2", re.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "First page",
                    disabled: !re.value,
                    onClick: R[1] || (R[1] = (Ve) => Me(-ee.value))
                  }, [...R[11] || (R[11] = [
                    n("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, Mf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 2,
                    type: "button",
                    class: m(["pl-2", O.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Previous page",
                    disabled: !O.value,
                    onClick: R[2] || (R[2] = (Ve) => Me(-S.value))
                  }, [...R[12] || (R[12] = [
                    n("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, Tf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 3,
                    type: "button",
                    class: m(["pl-2", Z.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Next page",
                    disabled: !Z.value,
                    onClick: R[3] || (R[3] = (Ve) => Me(S.value))
                  }, [...R[13] || (R[13] = [
                    n("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, Ff)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 4,
                    type: "button",
                    class: m(["pl-2", _.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Last page",
                    disabled: !_.value,
                    onClick: R[4] || (R[4] = (Ve) => Me(ee.value))
                  }, [...R[14] || (R[14] = [
                    n("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, Af)) : x("", !0)
                ]),
                e.showPagingInfo ? (a(), i("div", If, [
                  n("div", Df, [
                    b.value ? (a(), i("span", Of, "Querying...")) : x("", !0),
                    V.value.length ? (a(), i("span", Pf, [
                      R[15] || (R[15] = n("span", { class: "hidden xl:inline" }, " Showing Results ", -1)),
                      we(" " + M(h.value + 1) + " - " + M(Math.min(h.value + V.value.length, ee.value)) + " ", 1),
                      n("span", null, " of " + M(ee.value), 1)
                    ])) : g.value.completed ? (a(), i("span", jf, "No Results")) : x("", !0)
                  ])
                ])) : x("", !0),
                Y.value.Create && ue.value ? (a(), i("div", Bf, [
                  n("button", {
                    type: "button",
                    onClick: R[5] || (R[5] = (Ve) => ve()),
                    title: "modelTitle",
                    class: m(W(ge).toolbarButtonClass)
                  }, [
                    R[16] || (R[16] = n("svg", {
                      class: "w-5 h-5 mr-1 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        d: "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z",
                        fill: "currentColor"
                      })
                    ], -1)),
                    n("span", Hf, M(j.value), 1)
                  ], 2),
                  se.value ? (a(), X(Fe, {
                    key: 0,
                    ref_key: "createForm",
                    ref: fe,
                    type: Y.value.Create.request.name,
                    configure: e.configureField,
                    onDone: pe,
                    onSave: de
                  }, {
                    header: xe(() => [
                      H(G.$slots, "formheader", {
                        form: "create",
                        formInstance: fe.value,
                        apis: Y.value,
                        type: oe.value,
                        updateModel: ce
                      })
                    ]),
                    footer: xe(() => [
                      H(G.$slots, "formfooter", {
                        form: "create",
                        formInstance: fe.value,
                        apis: Y.value,
                        type: oe.value,
                        updateModel: ce
                      })
                    ]),
                    _: 3
                  }, 8, ["type", "configure"])) : x("", !0)
                ])) : x("", !0),
                F.value && e.showResetPreferences ? (a(), i("div", Rf, [
                  n("button", {
                    type: "button",
                    onClick: yt,
                    title: "Reset Preferences & Filters",
                    class: m(y.value)
                  }, [...R[17] || (R[17] = [
                    n("svg", {
                      class: "w-5 h-5",
                      xmlns: "http://www.w3.org/2000/svg",
                      "aria-hidden": "true",
                      viewBox: "0 0 24 24"
                    }, [
                      n("path", {
                        fill: "currentColor",
                        d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
                      })
                    ], -1)
                  ])], 2)
                ])) : x("", !0),
                n("div", Ef, [
                  e.showFiltersView && J.value > 0 ? (a(), i("div", Nf, [
                    n("button", {
                      type: "button",
                      onClick: R[6] || (R[6] = (Ve) => A.value = A.value == "filters" ? null : "filters"),
                      class: m(y.value),
                      "aria-expanded": "false"
                    }, [
                      R[20] || (R[20] = n("svg", {
                        class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
                        "aria-hidden": "true",
                        xmlns: "http://www.w3.org/2000/svg",
                        viewBox: "0 0 20 20",
                        fill: "currentColor"
                      }, [
                        n("path", {
                          "fill-rule": "evenodd",
                          d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
                          "clip-rule": "evenodd"
                        })
                      ], -1)),
                      n("span", zf, M(J.value) + " " + M(J.value == 1 ? "Filter" : "Filters"), 1),
                      A.value != "filters" ? (a(), i("svg", Uf, [...R[18] || (R[18] = [
                        n("path", {
                          "fill-rule": "evenodd",
                          d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
                          "clip-rule": "evenodd"
                        }, null, -1)
                      ])])) : (a(), i("svg", qf, [...R[19] || (R[19] = [
                        n("path", {
                          "fill-rule": "evenodd",
                          d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
                          "clip-rule": "evenodd"
                        }, null, -1)
                      ])]))
                    ], 2)
                  ])) : x("", !0)
                ])
              ]),
              A.value == "filters" ? (a(), X(jn, {
                key: 0,
                class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
                definitions: L.value,
                columns: ne.value,
                onDone: R[7] || (R[7] = (Ve) => A.value = null),
                onChange: ut
              }, null, 8, ["definitions", "columns"])) : x("", !0),
              p.value ? (a(), i("div", _f, [
                he(Pn, {
                  definitions: L.value,
                  column: p.value.column,
                  "top-left": p.value.topLeft,
                  onDone: et,
                  onSave: rt
                }, null, 8, ["definitions", "column", "top-left"])
              ])) : x("", !0),
              g.value.error ? (a(), X(De, {
                key: 2,
                status: g.value.error
              }, null, 8, ["status"])) : b.value ? (a(), X(_e, { key: 3 })) : (a(), i("div", Qf, [
                V.value.length ? (a(), i("div", Kf, [
                  he(ht, {
                    id: e.id,
                    items: V.value,
                    type: e.refInfo.model,
                    "selected-columns": U.value,
                    onFiltersChanged: Qe,
                    tableStyle: "fullWidth",
                    rowClass: le,
                    onRowSelected: Te,
                    onHeaderSelected: nt
                  }, cn({
                    header: xe(({ column: Ve, label: Rt }) => [
                      e.allowFiltering && (!l.canFilter || l.canFilter(Ve)) ? (a(), i("div", Zf, [
                        n("span", Wf, M(Rt), 1),
                        he(tt, {
                          column: ne.value.find((El) => El.name.toLowerCase() === Ve.toLowerCase()),
                          "is-open": p.value?.column.name === Ve
                        }, null, 8, ["column", "is-open"])
                      ])) : (a(), i("div", Gf, [
                        n("span", Jf, M(Rt), 1)
                      ]))
                    ]),
                    _: 2
                  }, [
                    Se(Object.keys(W(o)), (Ve) => ({
                      name: Ve,
                      fn: xe((Rt) => [
                        H(G.$slots, Ve, Cl(os(Rt)))
                      ])
                    }))
                  ]), 1032, ["id", "items", "type", "selected-columns"])
                ])) : x("", !0)
              ]))
            ])
          ]),
          _: 3
        }, 8, ["id"])) : x("", !0),
        k.value ? (a(), X(Bn, {
          key: 1,
          columns: K.value,
          prefs: w.value,
          onDone: R[8] || (R[8] = (Ve) => k.value = !1),
          onSave: vt
        }, null, 8, ["columns", "prefs"])) : x("", !0)
      ], 64);
    };
  }
}), Yf = { class: "sm:hidden" }, ev = ["for"], tv = ["id", "name"], lv = ["value"], nv = { class: "hidden sm:block" }, sv = { class: "border-b border-gray-200" }, av = {
  class: "-mb-px flex",
  "aria-label": "Tabs"
}, ov = ["onClick"], iv = /* @__PURE__ */ ie({
  __name: "Tabs",
  props: {
    tabs: {},
    id: { default: "tabs" },
    param: { default: "tab" },
    label: { type: Function, default: (e) => Pe(e) },
    selected: {},
    tabClass: {},
    bodyClass: { default: "p-4" },
    url: { type: Boolean, default: !0 },
    clearQuery: { type: Boolean, default: !1 }
  },
  setup(e) {
    const t = e, l = c(() => Object.keys(t.tabs)), s = (y) => t.label ? t.label(y) : Pe(y), o = c(() => t.id || "tabs"), r = c(() => t.param || "tab"), u = T();
    function d(y) {
      if (u.value = y, t.url) {
        const L = l.value[0];
        Tn({ tab: y === L ? void 0 : y }, t.clearQuery);
      }
    }
    function f(y) {
      return u.value === y;
    }
    const v = c(() => `${100 / Object.keys(t.tabs).length}%`);
    return Ye(() => {
      if (u.value = t.selected || Object.keys(t.tabs)[0], t.url) {
        const y = location.search ? location.search : location.hash.includes("?") ? "?" + hl(location.hash, "?") : "", w = Jl(y)[r.value];
        w && (u.value = w);
      }
    }), (y, L) => (a(), i("div", null, [
      n("div", Yf, [
        n("label", {
          for: o.value,
          class: "sr-only"
        }, "Select a tab", 8, ev),
        n("select", {
          id: o.value,
          name: o.value,
          class: "block w-full rounded-md border-gray-300 focus:border-indigo-500 focus:ring-indigo-500",
          onChange: L[0] || (L[0] = (w) => d(w.target?.value))
        }, [
          (a(!0), i($e, null, Se(l.value, (w) => (a(), i("option", {
            key: w,
            value: w
          }, M(s(w)), 9, lv))), 128))
        ], 40, tv)
      ]),
      n("div", nv, [
        n("div", sv, [
          n("nav", av, [
            (a(!0), i($e, null, Se(l.value, (w) => (a(), i("a", {
              href: "#",
              onClick: He((g) => d(w), ["prevent"]),
              style: un({ width: v.value }),
              class: m([f(w) ? "border-indigo-500 text-indigo-600 py-4 px-1 text-center border-b-2 font-medium text-sm" : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 py-4 px-1 text-center border-b-2 font-medium text-sm", e.tabClass])
            }, M(s(w)), 15, ov))), 256))
          ])
        ])
      ]),
      n("div", {
        class: m(e.bodyClass)
      }, [
        (a(), X(is(e.tabs[u.value])))
      ], 2)
    ]));
  }
}), rv = /* @__PURE__ */ ie({
  __name: "DarkModeToggle",
  setup(e) {
    const t = typeof document < "u" ? document.documentElement : null, l = () => !!t?.classList.contains("dark"), s = T(localStorage.getItem("color-scheme") == "dark");
    function o() {
      l() ? t?.classList.remove("dark") : t?.classList.add("dark"), s.value = l(), localStorage.setItem("color-scheme", s.value ? "dark" : "light");
    }
    return (r, u) => (a(), i("button", {
      type: "button",
      class: "bg-gray-200 dark:bg-gray-700 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black",
      role: "switch",
      "aria-checked": "false",
      onClick: u[0] || (u[0] = (d) => o())
    }, [
      n("span", {
        class: m(`${s.value ? "translate-x-0" : "translate-x-5"} pointer-events-none relative inline-block h-5 w-5 rounded-full bg-white dark:bg-black shadow transform ring-0 transition ease-in-out duration-200`)
      }, [
        n("span", {
          class: m(`${s.value ? "opacity-100 ease-in duration-200" : "opacity-0 ease-out duration-100"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, [...u[1] || (u[1] = [
          n("svg", {
            xmlns: "http://www.w3.org/2000/svg",
            class: "h-4 w-4 text-gray-400",
            preserveAspectRatio: "xMidYMid meet",
            viewBox: "0 0 32 32"
          }, [
            n("path", {
              fill: "currentColor",
              d: "M13.502 5.414a15.075 15.075 0 0 0 11.594 18.194a11.113 11.113 0 0 1-7.975 3.39c-.138 0-.278.005-.418 0a11.094 11.094 0 0 1-3.2-21.584M14.98 3a1.002 1.002 0 0 0-.175.016a13.096 13.096 0 0 0 1.825 25.981c.164.006.328 0 .49 0a13.072 13.072 0 0 0 10.703-5.555a1.01 1.01 0 0 0-.783-1.565A13.08 13.08 0 0 1 15.89 4.38A1.015 1.015 0 0 0 14.98 3Z"
            })
          ], -1)
        ])], 2),
        n("span", {
          class: m(`${s.value ? "opacity-0 ease-out duration-100" : "opacity-100 ease-in duration-200"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, [...u[2] || (u[2] = [
          n("svg", {
            xmlns: "http://www.w3.org/2000/svg",
            class: "h-4 w-4 text-indigo-600",
            preserveAspectRatio: "xMidYMid meet",
            viewBox: "0 0 32 32"
          }, [
            n("path", {
              fill: "currentColor",
              d: "M16 12.005a4 4 0 1 1-4 4a4.005 4.005 0 0 1 4-4m0-2a6 6 0 1 0 6 6a6 6 0 0 0-6-6ZM5.394 6.813L6.81 5.399l3.505 3.506L8.9 10.319zM2 15.005h5v2H2zm3.394 10.193L8.9 21.692l1.414 1.414l-3.505 3.506zM15 25.005h2v5h-2zm6.687-1.9l1.414-1.414l3.506 3.506l-1.414 1.414zm3.313-8.1h5v2h-5zm-3.313-6.101l3.506-3.506l1.414 1.414l-3.506 3.506zM15 2.005h2v5h-2z"
            })
          ], -1)
        ])], 2)
      ], 2)
    ]));
  }
}), uv = { key: 0 }, dv = {
  key: 1,
  class: "min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8"
}, cv = { class: "sm:mx-auto sm:w-full sm:max-w-md" }, fv = { class: "mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-gray-50" }, vv = {
  key: 0,
  class: "mt-4 text-center text-sm text-gray-600 dark:text-gray-300"
}, mv = { class: "relative z-0 inline-flex shadow-sm rounded-md" }, gv = ["onClick"], pv = { class: "mt-8 sm:mx-auto sm:w-full sm:max-w-md" }, yv = { class: "bg-white dark:bg-black py-8 px-4 shadow sm:rounded-lg sm:px-10" }, hv = { class: "mt-8" }, bv = {
  key: 1,
  class: "mt-6"
}, wv = { class: "mt-6 grid grid-cols-3 gap-3" }, xv = ["href", "title"], kv = {
  key: 1,
  class: "h-5 w-5 text-gray-700 dark:text-gray-200",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 32 32"
}, Cv = /* @__PURE__ */ ie({
  __name: "SignIn",
  props: {
    provider: {},
    title: { default: "Sign In" },
    tabs: { type: [Boolean, String], default: !0 },
    oauth: { type: [Boolean, String], default: !0 }
  },
  emits: ["login"],
  setup(e, { emit: t }) {
    const l = e, s = t, { getMetadata: o, createDto: r } = lt(), u = cl(), d = ze("client"), { signIn: f } = On(), v = o({ assert: !0 }), y = v.plugins.auth, L = document.baseURI, w = v.app.baseUrl, g = T(r("Authenticate")), h = T(new We()), b = T(l.provider);
    Ye(() => {
      y?.authProviders.map((O) => O.formLayout).filter((O) => O).forEach((O) => O.forEach(
        (Z) => g.value[Z.id] = Z.type === "checkbox" ? !1 : ""
      ));
    });
    const A = c(() => y?.authProviders.filter((O) => O.formLayout) || []), B = c(() => A.value[0] || {}), le = c(() => A.value[Math.max(A.value.length - 1, 0)] || {}), E = c(() => (b.value ? y?.authProviders.find((O) => O.name === b.value) : null) ?? B.value), I = (O) => O === !1 || O === "false";
    function K(O) {
      return O.label || O.navItem && O.navItem.label;
    }
    const U = c(() => (E.value?.formLayout || []).map((O) => Object.assign({}, O, {
      type: O.type?.toLowerCase(),
      autocomplete: O.autocomplete || (O.type?.toLowerCase() === "password" ? "current-password" : void 0) || (O.id.toLowerCase() === "username" ? "username" : void 0),
      css: Object.assign({ field: "col-span-12" }, O.css)
    }))), S = c(() => I(l.oauth) ? [] : y?.authProviders.filter((O) => O.type === "oauth") || []), V = c(() => {
      let O = Da(
        y?.authProviders.filter((_) => _.formLayout && _.formLayout.length > 0),
        (_, ne) => {
          let F = K(ne) || ot(ne.name);
          _[F] = ne.name === B.value.name ? "" : ne.name;
        }
      );
      const Z = E.value;
      return Z && I(l.tabs) && (O = { [K(Z) || ot(Z.name)]: Z }), O;
    }), ee = c(() => {
      let O = U.value.map((Z) => Z.id).filter((Z) => Z);
      return h.value.summaryMessage(O);
    });
    async function re() {
      if (g.value.provider = E.value.name, E.value.name === "authsecret" ? (d.headers.set("authsecret", g.value.authsecret), g.value = r("Authenticate")) : E.value.name === "basic" ? (d.setCredentials(g.value.UserName, g.value.Password), g.value = r("Authenticate"), g.value.UserName = null, g.value.Password = null) : (E.value.type === "Bearer" || E.value.name === "jwt") && (d.bearerToken = g.value.BearerToken, g.value = r("Authenticate")), h.value = await u.api(g.value), h.value.succeeded) {
        const O = h.value.response;
        f(O), s("login", O), h.value = new We(), g.value = r("Authenticate");
      }
    }
    return (O, Z) => {
      const _ = z("ErrorSummary"), ne = z("AutoFormFields"), F = z("PrimaryButton"), J = z("Icon"), C = pa("href");
      return W(y) ? (a(), i("div", dv, [
        n("div", cv, [
          n("h2", fv, M(e.title), 1),
          Object.keys(V.value).length > 1 ? (a(), i("p", vv, [
            n("span", mv, [
              (a(!0), i($e, null, Se(V.value, (P, Q) => It((a(), i("a", {
                onClick: (k) => b.value = P,
                class: m([
                  P === "" || P === le.value.name ? "rounded-l-md" : P === le.value.name ? "rounded-r-md -ml-px" : "-ml-px",
                  b.value === P ? "z-10 outline-none ring-1 ring-indigo-500 border-indigo-500" : "",
                  "cursor-pointer relative inline-flex items-center px-4 py-1 border border-gray-300 dark:border-gray-600 bg-white dark:bg-black text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-900"
                ])
              }, [
                we(M(Q), 1)
              ], 10, gv)), [
                [C, { provider: P }]
              ])), 256))
            ])
          ])) : x("", !0)
        ]),
        n("div", pv, [
          ee.value ? (a(), X(_, {
            key: 0,
            class: "mb-3",
            errorSummary: ee.value
          }, null, 8, ["errorSummary"])) : x("", !0),
          n("div", yv, [
            U.value.length ? (a(), i("form", {
              key: 0,
              onSubmit: He(re, ["prevent"])
            }, [
              he(ne, {
                modelValue: g.value,
                formLayout: U.value,
                api: h.value,
                hideSummary: !0,
                "divide-class": "",
                "space-class": "space-y-6"
              }, null, 8, ["modelValue", "formLayout", "api"]),
              n("div", hv, [
                he(F, { class: "w-full" }, {
                  default: xe(() => [...Z[0] || (Z[0] = [
                    we("Sign In", -1)
                  ])]),
                  _: 1
                })
              ])
            ], 32)) : x("", !0),
            S.value.length ? (a(), i("div", bv, [
              Z[2] || (Z[2] = Sl('<div class="relative"><div class="absolute inset-0 flex items-center"><div class="w-full border-t border-gray-300 dark:border-gray-600"></div></div><div class="relative flex justify-center text-sm"><span class="px-2 bg-white text-gray-500 dark:text-gray-400"> Or continue with </span></div></div>', 1)),
              n("div", wv, [
                (a(!0), i($e, null, Se(S.value, (P) => (a(), i("div", null, [
                  n("a", {
                    href: W(w) + P.navItem.href + "?continue=" + W(L),
                    title: K(P),
                    class: "w-full inline-flex justify-center py-2 px-4 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-black text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-900"
                  }, [
                    P.icon ? (a(), X(J, {
                      key: 0,
                      image: P.icon,
                      class: "h-5 w-5 text-gray-700 dark:text-gray-200"
                    }, null, 8, ["image"])) : (a(), i("svg", kv, [...Z[1] || (Z[1] = [
                      n("path", {
                        d: "M16 8a5 5 0 1 0 5 5a5 5 0 0 0-5-5z",
                        fill: "currentColor"
                      }, null, -1),
                      n("path", {
                        d: "M16 2a14 14 0 1 0 14 14A14.016 14.016 0 0 0 16 2zm7.992 22.926A5.002 5.002 0 0 0 19 20h-6a5.002 5.002 0 0 0-4.992 4.926a12 12 0 1 1 15.985 0z",
                        fill: "currentColor"
                      }, null, -1)
                    ])]))
                  ], 8, xv)
                ]))), 256))
              ])
            ])) : x("", !0)
          ])
        ])
      ])) : (a(), i("div", uv, "No Auth Plugin"));
    };
  }
}), $v = ["for"], Lv = {
  key: 1,
  class: "border border-gray-200 flex justify-between shadow-sm"
}, Vv = { class: "p-2 flex flex-wrap gap-x-4" }, Sv = {
  key: 0,
  class: "p-2 flex flex-wrap gap-x-4"
}, Mv = ["href"], Tv = { class: "" }, Fv = ["name", "id", "label", "value", "rows", "disabled"], Av = ["id"], Iv = ["id"], Je = "w-5 h-5 cursor-pointer select-none text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400", Dv = /* @__PURE__ */ ie({
  __name: "MarkdownInput",
  props: {
    status: {},
    id: {},
    inputClass: {},
    filterClass: {},
    label: {},
    labelClass: {},
    help: {},
    placeholder: {},
    modelValue: {},
    counter: { type: Boolean },
    rows: {},
    errorMessages: {},
    lang: {},
    autoFocus: { type: Boolean },
    disabled: { type: Boolean },
    helpUrl: { default: "https://guides.github.com/features/mastering-markdown/" },
    hide: {}
  },
  emits: ["update:modelValue", "close"],
  setup(e, { expose: t, emit: l }) {
    const s = e, o = l;
    let r = [], u = [], d = ze("ApiState", void 0);
    const f = c(() => pt.call({ responseStatus: s.status ?? d?.error.value }, s.id)), v = c(() => s.label ?? Pe(ot(s.id))), y = "bold,italics,link,image,blockquote,code,heading,orderedList,unorderedList,strikethrough,undo,redo,help".split(","), L = c(() => s.hide ? Ft(y, s.hide) : Ft(y, []));
    function w(k) {
      return L.value[k];
    }
    const g = c(() => kt([
      "shadow-sm font-mono" + at.base.replace("rounded-md", ""),
      f.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + at.valid,
      s.inputClass
    ], "MarkdownInput", s.filterClass)), h = T();
    t({ props: s, textarea: h, updateModelValue: b, selection: B, hasSelection: A, selectionInfo: le, insert: I, replace: E });
    function b(k) {
      o("update:modelValue", k);
    }
    function A() {
      return h.value.selectionStart !== h.value.selectionEnd;
    }
    function B() {
      const k = h.value;
      return k.value.substring(k.selectionStart, k.selectionEnd) || "";
    }
    function le() {
      const k = h.value, p = k.value, te = k.selectionStart, Y = p.substring(te, k.selectionEnd) || "", oe = p.substring(0, te), N = oe.lastIndexOf(`
`);
      return {
        value: p,
        sel: Y,
        selPos: te,
        beforeSel: oe,
        afterSel: p.substring(te),
        prevCRPos: N,
        beforeCR: N >= 0 ? oe.substring(0, N + 1) : "",
        afterCR: N >= 0 ? oe.substring(N + 1) : ""
      };
    }
    function E({ value: k, selectionStart: p, selectionEnd: te }) {
      te == null && (te = p), b(k), At(() => {
        h.value.focus(), h.value.setSelectionRange(p, te);
      });
    }
    function I(k, p, te = "", { selectionAtEnd: Y, offsetStart: oe, offsetEnd: N, filterValue: j, filterSelection: ue } = {}) {
      const fe = h.value;
      let se = fe.value, ve = fe.selectionEnd;
      r.push({ value: se, selectionStart: fe.selectionStart, selectionEnd: fe.selectionEnd }), u = [];
      const pe = fe.selectionStart, de = fe.selectionEnd;
      let ce = se.substring(0, pe), Le = se.substring(de);
      const Ie = k && ce.endsWith(k) && Le.startsWith(p);
      if (pe == de) {
        if (Ie ? (se = ce.substring(0, ce.length - k.length) + Le.substring(p.length), ve += -p.length) : (se = ce + k + te + p + Le, ve += k.length, oe = 0, N = te?.length || 0, Y && (ve += N, N = 0)), j) {
          var Me = { pos: ve };
          se = j(se, Me), ve = Me.pos;
        }
      } else {
        var Te = se.substring(pe, de);
        ue && (Te = ue(Te)), Ie ? (se = ce.substring(0, ce.length - k.length) + Te + Le.substring(p.length), oe = -Te.length - k.length, N = Te.length) : (se = ce + k + Te + p + Le, oe ? ve += (k + p).length : (ve = pe, oe = k.length, N = Te.length));
      }
      b(se), At(() => {
        fe.focus(), oe = ve + (oe || 0), N = (oe || 0) + (N || 0), fe.setSelectionRange(oe, N);
      });
    }
    const K = () => I("**", "**", "bold"), U = () => I("_", "_", "italics"), S = () => I("~~", "~~", "strikethrough"), V = () => I("[", "](https://)", "", { offsetStart: -9, offsetEnd: 8 }), ee = () => I(`
> `, `
`, "Blockquote", {}), re = () => I("![](", ")");
    function O(k) {
      const p = B();
      if (p && !k.shiftKey)
        I("`", "`", "code");
      else {
        const te = s.lang || "js";
        p.indexOf(`
`) === -1 ? I("\n```" + te + `
`, "\n```\n", "// code") : I("```" + te + `
`, "```\n", "");
      }
    }
    function Z() {
      if (A()) {
        let { sel: k, selPos: p, beforeSel: te, afterSel: Y, prevCRPos: oe, beforeCR: N, afterCR: j } = le();
        if (k.indexOf(`
`) === -1)
          I(`
 1. `, `
`);
        else if (!k.startsWith(" 1. ")) {
          let se = 1;
          I("", "", " - ", {
            selectionAtEnd: !0,
            filterSelection: (ve) => " 1. " + ve.replace(/\n$/, "").replace(/\n/g, (pe) => `
 ${++se}. `) + `
`
          });
        } else
          I("", "", "", {
            filterValue: (se, ve) => {
              if (oe >= 0) {
                let pe = j.replace(/^ - /, "");
                te = N + pe, ve.pos -= j.length - pe.length;
              }
              return te + Y;
            },
            filterSelection: (se) => se.replace(/^ 1. /g, "").replace(/\n \d+. /g, `
`)
          });
      } else
        I(`
 1. `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function _() {
      if (A()) {
        let { sel: k, selPos: p, beforeSel: te, afterSel: Y, prevCRPos: oe, beforeCR: N, afterCR: j } = le();
        k.indexOf(`
`) === -1 ? I(`
 - `, `
`) : !k.startsWith(" - ") ? I("", "", " - ", {
          selectionAtEnd: !0,
          filterSelection: (se) => " - " + se.replace(/\n$/, "").replace(/\n/g, `
 - `) + `
`
        }) : I("", "", "", {
          filterValue: (se, ve) => {
            if (oe >= 0) {
              let pe = j.replace(/^ - /, "");
              te = N + pe, ve.pos -= j.length - pe.length;
            }
            return te + Y;
          },
          filterSelection: (se) => se.replace(/^ - /g, "").replace(/\n - /g, `
`)
        });
      } else
        I(`
 - `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function ne() {
      const k = B(), p = k.indexOf(`
`) === -1;
      k ? p ? I(`
## `, `
`, "") : I("## ", "", "") : I(`
## `, `
`, "Heading", { offsetStart: -8, offsetEnd: 7 });
    }
    function F() {
      let { sel: k, selPos: p, beforeSel: te, afterSel: Y, prevCRPos: oe, beforeCR: N, afterCR: j } = le();
      !k.startsWith("//") && !j.startsWith("//") ? k ? I("", "", "//", {
        selectionAtEnd: !0,
        filterSelection: (fe) => "//" + fe.replace(/\n$/, "").replace(/\n/g, `
//`) + `
`
      }) : E({
        value: N + "//" + j + Y,
        selectionStart: p + 2
      }) : I("", "", "", {
        filterValue: (fe, se) => {
          if (oe >= 0) {
            let ve = j.replace(/^\/\//, "");
            te = N + ve, se.pos -= j.length - ve.length;
          }
          return te + Y;
        },
        filterSelection: (fe) => fe.replace(/^\/\//g, "").replace(/\n\/\//g, `
`)
      });
    }
    const J = () => I(`/*
`, `*/
`, "");
    function C() {
      if (r.length === 0) return !1;
      const k = h.value, p = r.pop();
      return u.push({ value: k.value, selectionStart: k.selectionStart, selectionEnd: k.selectionEnd }), E(p), !0;
    }
    function P() {
      if (u.length === 0) return !1;
      const k = h.value, p = u.pop();
      return r.push({ value: k.value, selectionStart: k.selectionStart, selectionEnd: k.selectionEnd }), E(p), !0;
    }
    const Q = () => null;
    return Ye(() => {
      r = [], u = [];
      const k = h.value;
      k.onkeydown = (p) => {
        if (p.key === "Escape" || p.keyCode === 27) {
          o("close");
          return;
        }
        const te = String.fromCharCode(p.keyCode).toLowerCase();
        te === "	" ? (!p.shiftKey ? I("", "", "    ", {
          selectionAtEnd: !0,
          filterSelection: (oe) => "    " + oe.replace(/\n$/, "").replace(/\n/g, `
    `) + `
`
        }) : I("", "", "", {
          filterValue: (oe, N) => {
            let { selPos: j, beforeSel: ue, afterSel: fe, prevCRPos: se, beforeCR: ve, afterCR: pe } = le();
            if (se >= 0) {
              let de = pe.replace(/\t/g, "    ").replace(/^ ? ? ? ?/, "");
              ue = ve + de, N.pos -= pe.length - de.length;
            }
            return ue + fe;
          },
          filterSelection: (oe) => oe.replace(/\t/g, "    ").replace(/^ ? ? ? ?/g, "").replace(/\n    /g, `
`)
        }), p.preventDefault()) : p.ctrlKey ? te === "z" ? p.shiftKey ? P() && p.preventDefault() : C() && p.preventDefault() : te === "b" && !p.shiftKey ? (K(), p.preventDefault()) : te === "h" && !p.shiftKey ? (ne(), p.preventDefault()) : te === "i" && !p.shiftKey ? (U(), p.preventDefault()) : te === "q" && !p.shiftKey ? (ee(), p.preventDefault()) : te === "k" ? p.shiftKey ? (re(), p.preventDefault()) : (V(), p.preventDefault()) : te === "," || p.key === "<" || p.key === ">" || p.keyCode === 188 ? (O(p), p.preventDefault()) : te === "/" || p.key === "/" ? (F(), p.preventDefault()) : (te === "?" || p.key === "?") && p.shiftKey && (J(), p.preventDefault()) : p.altKey && (p.key === "1" || p.key === "0" ? (Z(), p.preventDefault()) : p.key === "-" ? (_(), p.preventDefault()) : p.key === "s" && (S(), p.preventDefault()));
      };
    }), (k, p) => (a(), i("div", null, [
      H(k.$slots, "header", ke({
        inputElement: h.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, k.$attrs)),
      v.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: m(`mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, M(v.value), 11, $v)) : x("", !0),
      e.disabled ? x("", !0) : (a(), i("div", Lv, [
        n("div", Vv, [
          w("bold") ? (a(), i("svg", {
            key: 0,
            class: m(Je),
            onClick: K,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[1] || (p[1] = [
            n("title", null, "Bold text (CTRL+B)", -1),
            n("path", {
              fill: "currentColor",
              d: "M15.6 10.79c.97-.67 1.65-1.77 1.65-2.79c0-2.26-1.75-4-4-4H7v14h7.04c2.09 0 3.71-1.7 3.71-3.79c0-1.52-.86-2.82-2.15-3.42zM10 6.5h3c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5h-3v-3zm3.5 9H10v-3h3.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5z"
            }, null, -1)
          ])])) : x("", !0),
          w("italics") ? (a(), i("svg", {
            key: 1,
            class: m(Je),
            onClick: U,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[2] || (p[2] = [
            n("title", null, "Italics (CTRL+I)", -1),
            n("path", {
              fill: "currentColor",
              d: "M10 4v3h2.21l-3.42 8H6v3h8v-3h-2.21l3.42-8H18V4h-8z"
            }, null, -1)
          ])])) : x("", !0),
          w("link") ? (a(), i("svg", {
            key: 2,
            class: m(Je),
            onClick: V,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[3] || (p[3] = [
            n("title", null, "Insert Link (CTRL+K)", -1),
            n("path", {
              fill: "currentColor",
              d: "M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7a5 5 0 0 0-5 5a5 5 0 0 0 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1M8 13h8v-2H8v2m9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1c0 1.71-1.39 3.1-3.1 3.1h-4V17h4a5 5 0 0 0 5-5a5 5 0 0 0-5-5Z"
            }, null, -1)
          ])])) : x("", !0),
          w("blockquote") ? (a(), i("svg", {
            key: 3,
            class: m(Je),
            onClick: ee,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[4] || (p[4] = [
            n("title", null, "Blockquote (CTRL+Q)", -1),
            n("path", {
              fill: "currentColor",
              d: "m15 17l2-4h-4V6h7v7l-2 4h-3Zm-9 0l2-4H4V6h7v7l-2 4H6Z"
            }, null, -1)
          ])])) : x("", !0),
          w("image") ? (a(), i("svg", {
            key: 4,
            class: m(Je),
            onClick: re,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[5] || (p[5] = [
            n("title", null, "Insert Image (CTRL+SHIFT+L)", -1),
            n("path", {
              fill: "currentColor",
              d: "M2.992 21A.993.993 0 0 1 2 20.007V3.993A1 1 0 0 1 2.992 3h18.016c.548 0 .992.445.992.993v16.014a1 1 0 0 1-.992.993H2.992ZM20 15V5H4v14L14 9l6 6Zm0 2.828l-6-6L6.828 19H20v-1.172ZM8 11a2 2 0 1 1 0-4a2 2 0 0 1 0 4Z"
            }, null, -1)
          ])])) : x("", !0),
          w("code") ? (a(), i("svg", {
            key: 5,
            class: m(Je),
            onClick: O,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[6] || (p[6] = [
            n("title", null, "Insert Code (CTRL+<)", -1),
            n("path", {
              fill: "currentColor",
              d: "m8 18l-6-6l6-6l1.425 1.425l-4.6 4.6L9.4 16.6L8 18Zm8 0l-1.425-1.425l4.6-4.6L14.6 7.4L16 6l6 6l-6 6Z"
            }, null, -1)
          ])])) : x("", !0),
          w("heading") ? (a(), i("svg", {
            key: 6,
            class: m(Je),
            onClick: ne,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[7] || (p[7] = [
            n("title", null, "H2 Heading (CTRL+H)", -1),
            n("path", {
              fill: "currentColor",
              d: "M7 20V7H2V4h13v3h-5v13H7Zm9 0v-8h-3V9h9v3h-3v8h-3Z"
            }, null, -1)
          ])])) : x("", !0),
          w("orderedList") ? (a(), i("svg", {
            key: 7,
            class: m(Je),
            icon: "",
            onClick: Z,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[8] || (p[8] = [
            n("title", null, "Numbered List (ALT+1)", -1),
            n("path", {
              fill: "currentColor",
              d: "M3 22v-1.5h2.5v-.75H4v-1.5h1.5v-.75H3V16h3q.425 0 .713.288T7 17v1q0 .425-.288.713T6 19q.425 0 .713.288T7 20v1q0 .425-.288.713T6 22H3Zm0-7v-2.75q0-.425.288-.713T4 11.25h1.5v-.75H3V9h3q.425 0 .713.288T7 10v1.75q0 .425-.288.713T6 12.75H4.5v.75H7V15H3Zm1.5-7V3.5H3V2h3v6H4.5ZM9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9Z"
            }, null, -1)
          ])])) : x("", !0),
          w("unorderedList") ? (a(), i("svg", {
            key: 8,
            class: m(Je),
            onClick: _,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[9] || (p[9] = [
            n("title", null, "Bulleted List (ALT+-)", -1),
            n("path", {
              fill: "currentColor",
              d: "M9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9ZM5 20q-.825 0-1.413-.588T3 18q0-.825.588-1.413T5 16q.825 0 1.413.588T7 18q0 .825-.588 1.413T5 20Zm0-6q-.825 0-1.413-.588T3 12q0-.825.588-1.413T5 10q.825 0 1.413.588T7 12q0 .825-.588 1.413T5 14Zm0-6q-.825 0-1.413-.588T3 6q0-.825.588-1.413T5 4q.825 0 1.413.588T7 6q0 .825-.588 1.413T5 8Z"
            }, null, -1)
          ])])) : x("", !0),
          w("strikethrough") ? (a(), i("svg", {
            key: 9,
            class: m(Je),
            onClick: S,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[10] || (p[10] = [
            n("title", null, "Strike Through (ALT+S)", -1),
            n("path", {
              fill: "currentColor",
              d: "M10 19h4v-3h-4v3zM5 4v3h5v3h4V7h5V4H5zM3 14h18v-2H3v2z"
            }, null, -1)
          ])])) : x("", !0),
          w("undo") ? (a(), i("svg", {
            key: 10,
            class: m(Je),
            onClick: C,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[11] || (p[11] = [
            n("title", null, "Undo (CTRL+Z)", -1),
            n("path", {
              fill: "currentColor",
              d: "M12.5 8c-2.65 0-5.05.99-6.9 2.6L2 7v9h9l-3.62-3.62c1.39-1.16 3.16-1.88 5.12-1.88c3.54 0 6.55 2.31 7.6 5.5l2.37-.78C21.08 11.03 17.15 8 12.5 8z"
            }, null, -1)
          ])])) : x("", !0),
          w("redo") ? (a(), i("svg", {
            key: 11,
            class: m(Je),
            onClick: P,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...p[12] || (p[12] = [
            n("title", null, "Redo (CTRL+SHIFT+Z)", -1),
            n("path", {
              fill: "currentColor",
              d: "M18.4 10.6C16.55 8.99 14.15 8 11.5 8c-4.65 0-8.58 3.03-9.96 7.22L3.9 16a8.002 8.002 0 0 1 7.6-5.5c1.95 0 3.73.72 5.12 1.88L13 16h9V7l-3.6 3.6z"
            }, null, -1)
          ])])) : x("", !0),
          H(k.$slots, "toolbarbuttons", {
            instance: Oe()?.exposed
          })
        ]),
        w("help") && e.helpUrl ? (a(), i("div", Sv, [
          n("a", {
            title: "formatting help",
            target: "_blank",
            href: e.helpUrl,
            tabindex: "-1"
          }, [
            (a(), i("svg", {
              class: m(Je),
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, [...p[13] || (p[13] = [
              n("path", {
                fill: "currentColor",
                d: "M11 18h2v-2h-2v2zm1-16C6.48 2 2 6.48 2 12s4.48 10 10 10s10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8s8 3.59 8 8s-3.59 8-8 8zm0-14c-2.21 0-4 1.79-4 4h2c0-1.1.9-2 2-2s2 .9 2 2c0 2-3 1.75-3 5h2c0-2.25 3-2.5 3-5c0-2.21-1.79-4-4-4z"
              }, null, -1)
            ])]))
          ], 8, Mv)
        ])) : x("", !0)
      ])),
      n("div", Tv, [
        n("textarea", {
          ref_key: "txt",
          ref: h,
          name: e.id,
          id: e.id,
          class: m(g.value),
          label: e.label,
          value: e.modelValue,
          rows: e.rows || 6,
          disabled: e.disabled,
          onInput: p[0] || (p[0] = (te) => b(te.target?.value || "")),
          onKeydown: as(Q, ["tab"])
        }, null, 42, Fv)
      ]),
      f.value ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, M(f.value), 9, Av)) : e.help ? (a(), i("p", {
        key: 3,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, M(e.help), 9, Iv)) : x("", !0),
      H(k.$slots, "footer", ke({
        inputElement: h.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, k.$attrs))
    ]));
  }
}), Ov = {
  key: 0,
  class: "relative z-10 lg:hidden",
  role: "dialog",
  "aria-modal": "true"
}, Pv = { class: "fixed inset-0 flex" }, jv = { class: "flex grow flex-col gap-y-5 overflow-y-auto bg-white dark:bg-black px-6 pb-2" }, Bv = { class: "hidden lg:fixed lg:inset-y-0 lg:z-10 lg:flex lg:w-72 lg:flex-col" }, Hv = { class: "flex grow flex-col gap-y-5 overflow-y-auto border-r border-gray-200 dark:border-gray-700 bg-white dark:bg-black px-6" }, Rv = {
  class: /* @__PURE__ */ m(["sticky top-0 flex items-center gap-x-6 bg-white dark:bg-black px-4 py-4 shadow-sm sm:px-6 lg:hidden"])
}, Ev = /* @__PURE__ */ ie({
  __name: "SidebarLayout",
  setup(e, { expose: t }) {
    const { transition: l } = Xs(), s = T(!0), o = T(""), r = {
      entering: { cls: "transition-opacity ease-linear duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "transition-opacity ease-linear duration-300", from: "opacity-100", to: "opacity-0" }
    }, u = T(""), d = {
      entering: { cls: "transition ease-in-out duration-300 transform", from: "-translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transition ease-in-out duration-300 transform", from: "translate-x-0", to: "-translate-x-full" }
    }, f = T(""), v = {
      entering: { cls: "ease-in-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in-out duration-300", from: "opacity-100", to: "opacity-0" }
    };
    function y(g) {
      l(r, o, g), l(d, u, g), l(v, f, g), setTimeout(() => s.value = g, 300);
    }
    function L() {
      y(!0);
    }
    function w() {
      y(!1);
    }
    return t({ show: L, hide: w, toggle: y }), (g, h) => (a(), i("div", null, [
      s.value ? (a(), i("div", Ov, [
        n("div", {
          class: m(["fixed inset-0 bg-gray-900/80", o.value])
        }, null, 2),
        n("div", Pv, [
          n("div", {
            class: m(["relative mr-16 flex w-full max-w-xs flex-1", u.value])
          }, [
            n("div", {
              class: m(["absolute left-full top-0 flex w-16 justify-center pt-5", f.value])
            }, [
              n("button", {
                type: "button",
                onClick: w,
                class: "-m-2.5 p-2.5"
              }, [...h[0] || (h[0] = [
                n("span", { class: "sr-only" }, "Close sidebar", -1),
                n("svg", {
                  class: "h-6 w-6 text-white dark:text-black",
                  fill: "none",
                  viewBox: "0 0 24 24",
                  "stroke-width": "1.5",
                  stroke: "currentColor",
                  "aria-hidden": "true"
                }, [
                  n("path", {
                    "stroke-linecap": "round",
                    "stroke-linejoin": "round",
                    d: "M6 18L18 6M6 6l12 12"
                  })
                ], -1)
              ])])
            ], 2),
            n("div", jv, [
              H(g.$slots, "default")
            ])
          ], 2)
        ])
      ])) : x("", !0),
      n("div", Bv, [
        n("div", Hv, [
          H(g.$slots, "default")
        ])
      ]),
      n("div", Rv, [
        n("button", {
          type: "button",
          onClick: L,
          class: "-m-2.5 p-2.5 text-gray-700 dark:text-gray-200 lg:hidden"
        }, [...h[1] || (h[1] = [
          n("span", { class: "sr-only" }, "Open sidebar", -1),
          n("svg", {
            class: "h-6 w-6",
            fill: "none",
            viewBox: "0 0 24 24",
            "stroke-width": "1.5",
            stroke: "currentColor",
            "aria-hidden": "true"
          }, [
            n("path", {
              "stroke-linecap": "round",
              "stroke-linejoin": "round",
              d: "M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
            })
          ], -1)
        ])]),
        H(g.$slots, "mobiletitlebar")
      ])
    ]));
  }
}), Nv = Ro, zv = Ko, Uv = Jo, qv = Yo, _v = ta, Qv = li, Kv = ai, Zv = ri, Wv = ci, Gv = fi, Jv = yi, Xv = xi, Yv = $i, em = Di, tm = iu, lm = fu, nm = jn, sm = Pn, am = Bn, om = la, im = vu, rm = wu, um = Su, dm = Du, cm = Nu, fm = ad, vm = Vd, mm = jd, gm = Rd, pm = Ed, ym = Xd, hm = Yd, bm = hc, wm = Bc, xm = l0, km = b0, Cm = w0, $m = $0, Lm = T0, Vm = F0, Sm = j0, Mm = E0, Tm = U0, Fm = X0, Am = tf, Im = pf, Dm = xf, Om = Xf, Pm = iv, jm = rv, Bm = Cv, Hm = Dv, Rm = Ev, Em = {
  Alert: Nv,
  AlertSuccess: zv,
  ErrorSummary: Uv,
  InputDescription: qv,
  Icon: _v,
  Loading: Qv,
  OutlineButton: Kv,
  PrimaryButton: Zv,
  SecondaryButton: Wv,
  TextLink: Gv,
  Breadcrumbs: Jv,
  Breadcrumb: Xv,
  NavList: Yv,
  NavListItem: em,
  AutoQueryGrid: tm,
  SettingsIcons: lm,
  FilterViews: nm,
  FilterColumn: sm,
  QueryPrefs: am,
  EnsureAccess: om,
  EnsureAccessDialog: im,
  TextInput: rm,
  TextareaInput: um,
  SelectInput: dm,
  CheckboxInput: cm,
  TagInput: fm,
  FileInput: vm,
  Autocomplete: mm,
  Combobox: gm,
  DynamicInput: pm,
  LookupInput: ym,
  AutoFormFields: hm,
  AutoForm: bm,
  AutoCreateForm: wm,
  AutoEditForm: xm,
  AutoViewForm: km,
  ConfirmDelete: Cm,
  FormLoading: $m,
  DataGrid: Lm,
  CellFormat: Vm,
  PreviewFormat: Sm,
  HtmlFormat: Mm,
  MarkupFormat: Tm,
  MarkupModel: Fm,
  CloseButton: Am,
  SlideOver: Im,
  ModalDialog: Dm,
  ModalLookup: Om,
  Tabs: Pm,
  DarkModeToggle: jm,
  SignIn: Bm,
  MarkdownInput: Hm,
  SidebarLayout: Rm
}, Wl = Em || {}, _m = {
  install(e) {
    Object.keys(Wl).forEach((l) => {
      e.component(l, Wl[l]);
    });
    function t(l) {
      const o = Object.keys(l).filter((r) => l[r]).map((r) => `${encodeURIComponent(r)}=${encodeURIComponent(l[r])}`).join("&");
      return o ? "?" + o : "./";
    }
    e.directive("href", function(l, s) {
      l.href = t(s.value), l.onclick = (o) => {
        o.preventDefault(), history.pushState(s.value, "", t(s.value));
      };
    });
  },
  component(e, t) {
    return e ? t ? q.components[e] = t : q.components[e] || Wl[e] || null : null;
  }
};
export {
  Nv as Alert,
  zv as AlertSuccess,
  wm as AutoCreateForm,
  xm as AutoEditForm,
  bm as AutoForm,
  hm as AutoFormFields,
  tm as AutoQueryGrid,
  km as AutoViewForm,
  mm as Autocomplete,
  Xv as Breadcrumb,
  Jv as Breadcrumbs,
  Vm as CellFormat,
  cm as CheckboxInput,
  Am as CloseButton,
  gm as Combobox,
  Em as Components,
  Cm as ConfirmDelete,
  jm as DarkModeToggle,
  Lm as DataGrid,
  pm as DynamicInput,
  om as EnsureAccess,
  im as EnsureAccessDialog,
  Uv as ErrorSummary,
  vm as FileInput,
  sm as FilterColumn,
  nm as FilterViews,
  $m as FormLoading,
  Mm as HtmlFormat,
  _v as Icon,
  qv as InputDescription,
  Qv as Loading,
  ym as LookupInput,
  Hm as MarkdownInput,
  Tm as MarkupFormat,
  Fm as MarkupModel,
  Ra as MetadataApp,
  Dm as ModalDialog,
  Om as ModalLookup,
  Yv as NavList,
  em as NavListItem,
  Kv as OutlineButton,
  Sm as PreviewFormat,
  Zv as PrimaryButton,
  am as QueryPrefs,
  Wv as SecondaryButton,
  dm as SelectInput,
  lm as SettingsIcons,
  Rm as SidebarLayout,
  Bm as SignIn,
  Im as SlideOver,
  Pm as Tabs,
  fm as TagInput,
  rm as TextInput,
  Gv as TextLink,
  um as TextareaInput,
  qm as css,
  _m as default,
  On as useAuth,
  cl as useClient,
  Bt as useConfig,
  Ha as useFiles,
  Um as useFormatters,
  lt as useMetadata,
  Xs as useUtils
};
