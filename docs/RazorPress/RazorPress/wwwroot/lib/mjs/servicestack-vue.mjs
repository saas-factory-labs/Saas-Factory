import { computed as c, inject as Ne, defineComponent as ue, createElementBlock as i, openBlock as a, mergeProps as Ce, withModifiers as He, renderSlot as O, ref as A, shallowRef as Zl, unref as G, isRef as en, nextTick as wt, provide as ul, watchEffect as Cl, normalizeClass as g, createElementVNode as s, createCommentVNode as x, toDisplayString as T, h as St, resolveComponent as U, createBlock as X, withCtx as we, useAttrs as ca, createVNode as pe, createTextVNode as he, normalizeStyle as is, createStaticVNode as Sl, Fragment as Ve, renderList as Se, withDirectives as Pt, vModelCheckbox as rs, withKeys as tn, vModelSelect as fa, markRaw as va, useSlots as Tl, getCurrentInstance as Pe, onMounted as tt, createSlots as us, normalizeProps as $l, guardReactiveProps as ln, vModelDynamic as ma, onUnmounted as jt, watch as kt, vModelText as ga, resolveDynamicComponent as sn, resolveDirective as pa } from "vue";
import { lastRightPart as Dt, leftPart as Fl, map as Ee, toDate as gt, mapGet as ye, toCamelCase as ya, toDateTime as ha, chop as ba, isDate as Al, dateFmt as wa, fromXsdDuration as nn, timeFmt12 as xa, omit as ft, appendQueryString as Zt, indexOfAny as ka, apiValue as Ca, enc as Wl, createBus as $a, toKebabCase as Qs, ApiResult as Ge, nameOf as La, lastLeftPart as an, setQueryString as Va, toTime as Ma, ResponseStatus as Ql, ResponseError as Ks, sanitize as Sa, errorResponseExcept as Ta, humanize as De, delaySet as on, rightPart as bl, queryString as Gl, combinePaths as Fa, toPascalCase as ut, errorResponse as pt, trimEnd as Aa, $1 as Ll, HttpMethods as ds, omitEmpty as Ia, uniqueKeys as Jl, humanify as rn, each as Pa } from "@servicestack/client";
const un = "png,jpg,jpeg,jfif,gif,svg,webp".split(","), dn = {
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
}, _s = Object.keys(dn), mt = (e, t) => `<svg xmlns='http://www.w3.org/2000/svg' aria-hidden='true' role='img' preserveAspectRatio='xMidYMid meet' viewBox='${e}'>${t}</svg>`, wl = {
  img: mt("4 4 16 16", "<path fill='currentColor' d='M20 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2zm-2 0H6v6.38l2.19-2.19l5.23 5.23l1-1a1.59 1.59 0 0 1 2.11.11L18 16V6zm-5 3.5a1.5 1.5 0 1 1 3 0a1.5 1.5 0 0 1-3 0z'/>"),
  vid: mt("0 0 24 24", "<path fill='currentColor' d='m14 2l6 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8m4 18V9h-5V4H6v16h12m-2-2l-2.5-1.7V18H8v-5h5.5v1.7L16 13v5Z'/>"),
  aud: mt("0 0 24 24", "<path fill='currentColor' d='M14 2H6c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8l-6-6zM6 20V4h7v5h5v11H6zm10-9h-4v3.88a2.247 2.247 0 0 0-3.5 1.87c0 1.24 1.01 2.25 2.25 2.25S13 17.99 13 16.75V13h3v-2z'/>"),
  ppt: mt("0 0 48 48", "<g fill='none' stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='4'><path d='M4 8h40'/><path d='M8 8h32v26H8V8Z' clip-rule='evenodd'/><path d='m22 16l5 5l-5 5m-6 16l8-8l8 8'/></g>"),
  xls: mt("0 0 256 256", "<path fill='currentColor' d='M200 26H72a14 14 0 0 0-14 14v26H40a14 14 0 0 0-14 14v96a14 14 0 0 0 14 14h18v26a14 14 0 0 0 14 14h128a14 14 0 0 0 14-14V40a14 14 0 0 0-14-14Zm-42 76h44v52h-44Zm44-62v50h-44V80a14 14 0 0 0-14-14h-2V38h58a2 2 0 0 1 2 2ZM70 40a2 2 0 0 1 2-2h58v28H70ZM38 176V80a2 2 0 0 1 2-2h104a2 2 0 0 1 2 2v96a2 2 0 0 1-2 2H40a2 2 0 0 1-2-2Zm32 40v-26h60v28H72a2 2 0 0 1-2-2Zm130 2h-58v-28h2a14 14 0 0 0 14-14v-10h44v50a2 2 0 0 1-2 2ZM69.2 148.4L84.5 128l-15.3-20.4a6 6 0 1 1 9.6-7.2L92 118l13.2-17.6a6 6 0 0 1 9.6 7.2L99.5 128l15.3 20.4a6 6 0 0 1-9.6 7.2L92 138l-13.2 17.6a6 6 0 1 1-9.6-7.2Z'/>"),
  doc: mt("0 0 32 32", "<path fill='currentColor' d='M26 30H11a2.002 2.002 0 0 1-2-2v-6h2v6h15V6h-9V4h9a2.002 2.002 0 0 1 2 2v22a2.002 2.002 0 0 1-2 2Z'/><path fill='currentColor' d='M17 10h7v2h-7zm-1 5h8v2h-8zm-1 5h9v2h-9zm-6-1a5.005 5.005 0 0 1-5-5V3h2v11a3 3 0 0 0 6 0V5a1 1 0 0 0-2 0v10H8V5a3 3 0 0 1 6 0v9a5.005 5.005 0 0 1-5 5z'/>"),
  zip: mt("0 0 16 16", "<g fill='currentColor'><path d='M6.5 7.5a1 1 0 0 1 1-1h1a1 1 0 0 1 1 1v.938l.4 1.599a1 1 0 0 1-.416 1.074l-.93.62a1 1 0 0 1-1.109 0l-.93-.62a1 1 0 0 1-.415-1.074l.4-1.599V7.5zm2 0h-1v.938a1 1 0 0 1-.03.243l-.4 1.598l.93.62l.93-.62l-.4-1.598a1 1 0 0 1-.03-.243V7.5z'/><path d='M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2zm5.5-1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H9v1H8v1h1v1H8v1h1v1H7.5V5h-1V4h1V3h-1V2h1V1z'/></g>"),
  exe: mt("0 0 16 16", "<path fill='currentColor' fill-rule='evenodd' d='M14 4.5V14a2 2 0 0 1-2 2h-1v-1h1a1 1 0 0 0 1-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5L14 4.5ZM2.575 15.202H.785v-1.073H2.47v-.606H.785v-1.025h1.79v-.648H0v3.999h2.575v-.647ZM6.31 11.85h-.893l-.823 1.439h-.036l-.832-1.439h-.931l1.227 1.983l-1.239 2.016h.861l.853-1.415h.035l.85 1.415h.908l-1.254-1.992L6.31 11.85Zm1.025 3.352h1.79v.647H6.548V11.85h2.576v.648h-1.79v1.025h1.684v.606H7.334v1.073Z'/>"),
  att: mt("0 0 24 24", "<path fill='currentColor' d='M14 0a5 5 0 0 1 5 5v12a7 7 0 1 1-14 0V9h2v8a5 5 0 0 0 10 0V5a3 3 0 1 0-6 0v12a1 1 0 1 0 2 0V6h2v11a3 3 0 1 1-6 0V5a5 5 0 0 1 5-5Z'/>")
}, Da = /[\r\n%#()<>?[\\\]^`{|}]/g, Zs = 1024, Ba = ["Bytes", "KB", "MB", "GB", "TB"], ja = (() => {
  const e = "application/", t = e + "vnd.openxmlformats-officedocument.", l = "image/", n = "text/", o = "audio/", r = "video/", u = {
    jpg: l + "jpeg",
    tif: l + "tiff",
    svg: l + "svg+xml",
    ico: l + "x-icon",
    ts: n + "typescript",
    py: n + "x-python",
    sh: n + "x-sh",
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
  function d(v, p) {
    v.split(",").forEach(($) => u[$] = p);
  }
  function f(v, p) {
    v.split(",").forEach(($) => u[$] = p($));
  }
  return f("jpeg,gif,png,tiff,bmp,webp", (v) => l + v), f("jsx,csv,css", (v) => n + v), f("aac,ac3,aiff,m4a,m4b,m4p,mid,midi,wav", (v) => o + v), f("3gpp,avi,dv,divx,ogg,mp4,webm", (v) => r + v), f("rtf,pdf", (v) => e + v), d("htm,html,shtm", n + "html"), d("js,mjs,cjs", n + "javascript"), d("yml,yaml", e + "yaml"), d("bat,cmd", e + "bat"), d("xml,csproj,fsproj,vbproj", n + "xml"), d("txt,ps1", n + "plain"), d("qt,mov", r + "quicktime"), d("doc,dot", e + "msword"), d("xls,xlt,xla", e + "excel"), d("ppt,oit,pps,ppa", e + "vnd.ms-powerpoint"), d("cer,crt,der", e + "x-x509-ca-cert"), d("gz,tgz,zip,rar,lzh,z", e + "x-compressed"), d("aaf,aca,asd,bin,cab,chm,class,cur,db,dat,deploy,dll,dsp,exe,fla,ics,inf,mix,msi,mso,obj,ocx,prm,prx,psd,psp,qxd,sea,snp,so,sqlite,toc,ttf,u32,xmp,xsn,xtp", e + "octet-stream"), u;
})();
let Xl = [];
function cn(e) {
  return e = e.replace(/"/g, "'"), e = e.replace(/>\s+</g, "><"), e = e.replace(/\s{2,}/g, " "), e.replace(Da, encodeURIComponent);
}
function cs(e) {
  return "data:image/svg+xml;utf8," + cn(e);
}
function fn(e) {
  let t = URL.createObjectURL(e);
  return Xl.push(t), t;
}
function vn() {
  Xl.forEach((e) => {
    try {
      URL.revokeObjectURL(e);
    } catch (t) {
      console.error("URL.revokeObjectURL", t);
    }
  }), Xl = [];
}
function fs(e) {
  if (!e) return null;
  let t = Fl(e, "?");
  return Dt(t, "/");
}
function dl(e) {
  let t = fs(e);
  return t == null || t.indexOf(".") === -1 ? null : Dt(t, ".").toLowerCase();
}
function vs(e) {
  let t = dl(e.name);
  return t && un.indexOf(t) >= 0 ? fn(e) : Tt(e.name);
}
function ms(e) {
  if (!e) return !1;
  if (e.startsWith("blob:") || e.startsWith("data:"))
    return !0;
  let t = dl(e);
  return t && un.indexOf(t) >= 0 || !1;
}
function Tt(e) {
  if (!e) return null;
  let t = dl(e);
  return t == null || ms(e) ? e : ll(t) || cs(wl.doc);
}
function ll(e) {
  let t = mn(e);
  return t && cs(t) || null;
}
function mn(e) {
  if (wl[e])
    return wl[e];
  for (let t = 0; t < _s.length; t++) {
    let l = _s[t];
    if (dn[l].indexOf(e) >= 0)
      return wl[l];
  }
  return null;
}
function gs(e, t = 2) {
  if (e === 0) return "0 Bytes";
  const l = t < 0 ? 0 : t, n = Math.floor(Math.log(e) / Math.log(Zs));
  return parseFloat((e / Math.pow(Zs, n)).toFixed(l)) + " " + Ba[n];
}
function Oa(e) {
  return e.files && Array.from(e.files).map((t) => ({ fileName: t.name, contentLength: t.size, filePath: vs(t) }));
}
function Il(e, t) {
  e.onerror = null, e.src = ps(e.src, t) || "";
}
function ps(e, t) {
  return ll(Dt(e, ".").toLowerCase()) || (t ? ll(t) || t : null) || ll("doc");
}
function Yl(e) {
  if (!e)
    throw new Error("fileNameOrExt required");
  const t = Dt(e, ".").toLowerCase();
  return ja[t] || "application/" + t;
}
function Ha() {
  return {
    extSvg: mn,
    extSrc: ll,
    getExt: dl,
    encodeSvg: cn,
    canPreview: ms,
    getFileName: fs,
    getMimeType: Yl,
    formatBytes: gs,
    filePathUri: Tt,
    svgToDataUri: cs,
    fileImageUri: vs,
    objectUrl: fn,
    flush: vn,
    inputFiles: Oa,
    iconOnError: Il,
    iconFallbackSrc: ps
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
const Wt = "/metadata/app.json", Ea = {
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
}, es = {
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
[...Object.keys(es), ...Object.values(es)];
const za = {
  String: "string",
  Boolean: "bool",
  ...es
};
function pl(e) {
  return za[e] || e;
}
function gn(e, t) {
  return e ? (t || (t = []), e === "Nullable`1" ? pl(t[0]) + "?" : e.endsWith("[]") ? `List<${pl(e.substring(0, e.length - 2))}>` : t.length === 0 ? pl(e) : Fl(pl(e), "`") + "<" + t.join(",") + ">") : "";
}
function Ua(e) {
  return e && gn(e.name, e.genericArgs);
}
class bt {
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
    Re.isQueryInto(t) && !this.QueryInto ? this.QueryInto = t : Re.isQuery(t) && !this.Query ? this.Query = t : Re.isCreate(t) && !this.Create ? this.Create = t : Re.isUpdate(t) && !this.Update ? this.Update = t : Re.isPatch(t) && !this.Patch ? this.Patch = t : Re.isDelete(t) && !this.Delete && (this.Delete = t);
  }
  static from(t) {
    const l = new bt();
    return t.forEach((n) => {
      l.add(n);
    }), l;
  }
  static forType(t, l) {
    let n = new bt();
    if (N.config.apisResolver && t) {
      const o = N.config.apisResolver(t, l);
      o && (n.Query = o.Query, n.QueryInto = o.QueryInto, n.Create = o.Create, n.Update = o.Update, n.Patch = o.Patch, n.Delete = o.Delete);
    }
    return t && (l ??= N.metadata.value?.api, l?.operations.forEach((o) => {
      o.dataModel?.name == t && n.add(o);
    })), n;
  }
  /** Build a type context bundle for AQ components */
  static createContext(t) {
    const l = t.id || "AutoQueryGrid";
    let n = t.type;
    const o = _t(t.apis);
    !n && t.apis && o.length > 0 && (n = It(o[0])?.dataModel?.name);
    const r = t.metadataApi ?? N.metadata.value?.api, u = t.filterDefinitions ?? (N.metadata.value?.plugins?.autoQuery?.viewerConventions || Mn), d = Ht(n);
    console.log("createContext", l, n, t.apis, d, o);
    const f = o.length > 0 ? bt.from(o.map((M) => It(M)).filter((M) => M != null).map((M) => M)) : bt.forType(d, r), v = d || f.AnyQuery?.dataModel?.name, p = v ?? "", $ = Je(v), b = et($), y = Je(v), h = et(y), k = Jt(y), V = o.map((M) => It(M) == null ? M : null).filter((M) => M != null);
    return {
      typeName: d,
      dataModel: y,
      dataModelName: v,
      viewModel: $,
      viewModelProps: b,
      dataModelProps: h,
      dataModelPrimaryKey: k,
      apis: f,
      opNames: o,
      invalidApis: V,
      metadataApi: r,
      filterDefinitions: u,
      prefsCacheKey: () => `${l}/ApiPrefs/${p}`,
      columnCacheKey: (M) => `Column/${l}:${p}.${M}`
    };
  }
}
const Re = {
  Create: "ICreateDb`1",
  Update: "IUpdateDb`1",
  Patch: "IPatchDb`1",
  Delete: "IDeleteDb`1",
  AnyRead: ["QueryDb`1", "QueryDb`2"],
  AnyWrite: ["ICreateDb`1", "IUpdateDb`1", "IPatchDb`1", "IDeleteDb`1"],
  isAnyQuery: (e) => Ee(e.request.inherits, (t) => Re.AnyRead.indexOf(t.name) >= 0),
  isQuery: (e) => Ee(e.request.inherits, (t) => t.name === "QueryDb`1"),
  isQueryInto: (e) => Ee(e.request.inherits, (t) => t.name === "QueryDb`2"),
  isCrud: (e) => e.request.implements?.some((t) => Re.AnyWrite.indexOf(t.name) >= 0),
  isCreate: (e) => yl(e, Re.Create),
  isUpdate: (e) => yl(e, Re.Update),
  isPatch: (e) => yl(e, Re.Patch),
  isDelete: (e) => yl(e, Re.Delete),
  model: (e) => e ? Ee(e.inherits, (t) => Re.AnyRead.indexOf(t.name) >= 0) ? e.inherits?.genericArgs[0] : e.implements?.find((t) => Re.AnyWrite.indexOf(t.name) >= 0)?.genericArgs[0] : null
};
function qa(e) {
  return e.input?.type || Pl(ys(e));
}
function pn(e) {
  return e.endsWith("?") ? ba(e, 1) : e;
}
function Pl(e) {
  return Ea[pn(e)];
}
function Qa(e) {
  return e && Na[e] || "String";
}
function ys(e) {
  return e.type === "Nullable`1" ? e.genericArgs[0] : e.type;
}
function ts(e) {
  return e && Pl(e) == "number" || !1;
}
function yn(e) {
  return e && e.toLowerCase() == "string" || !1;
}
function Ka(e) {
  return e == "List`1" || e.startsWith("List<") || e.endsWith("[]");
}
function hn(e) {
  if (!e?.type) return !1;
  const t = ys(e);
  return e.isValueType && t.indexOf("`") == -1 || e.isEnum ? !1 : Pl(e.type) == null;
}
function bn(e) {
  if (!e?.type) return !1;
  const t = ys(e);
  if (e.isValueType && t.indexOf("`") == -1 || e.isEnum) return !0;
  const l = e.input?.type;
  return l && (l == "hidden" || l == "file" || l == "tag" || l == "combobox" || N.components?.[l]) ? !0 : Pl(e.type) != null;
}
function al(e, t) {
  let l = typeof e == "string" ? It(e) : e;
  l || (console.warn(`Metadata not found for: ${e}`), l = { request: { name: e } });
  let n = (
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
        return l.returnsVoid ? void 0 : new n();
      }, r.prototype.getTypeName = function() {
        return l.request.name;
      }, r.prototype.getMethod = function() {
        return l.method || "POST";
      }, r;
    })()
  );
  return new o(t);
}
function _a(e, t, l = {}) {
  let n = (
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
        return typeof l.createResponse == "function" ? l.createResponse() : new n();
      }, r.prototype.getTypeName = function() {
        return e;
      }, r.prototype.getMethod = function() {
        return l.method || "POST";
      }, r;
    })()
  );
  return new o(t);
}
function sl(e, t) {
  return e ? (Object.keys(e).forEach((l) => {
    let n = e[l];
    typeof n == "string" ? n.startsWith("/Date") && (e[l] = jl(gt(n))) : n != null && typeof n == "object" && (Array.isArray(n) ? e[l] = Array.from(n) : e[l] = Object.assign({}, n));
  }), e) : {};
}
function Za(e, t) {
  let l = {};
  return Array.from(e.elements).forEach((n) => {
    let o = n;
    if (!o.id || o.value == null || o.value === "") return;
    const r = o.id.toLowerCase(), u = t && t.find((p) => p.name.toLowerCase() == r);
    let d = u?.type, f = u?.genericArgs?.[0], v = o.type === "checkbox" ? o.checked : o.value;
    ts(d) ? v = Number(v) : d === "List`1" && typeof v == "string" && (v = v.split(",").map((p) => ts(f) ? Number(p) : p)), l[o.id] = v;
  }), l;
}
function hs(e) {
  return e?.api?.operations && e.api.operations.length > 0;
}
function Wa(e) {
  if (!bs() && e?.assert && !N.metadata.value)
    throw new Error("useMetadata() not configured, see: https://docs.servicestack.net/vue/use-metadata");
  return N.metadata.value;
}
function ol(e) {
  return e && hs(e) ? (e.date = ha(/* @__PURE__ */ new Date()), N.metadata.value = e, typeof localStorage < "u" && localStorage.setItem(Wt, JSON.stringify(e)), !0) : !1;
}
function Ga() {
  N.metadata.value = null, typeof localStorage < "u" && localStorage.removeItem(Wt);
}
function bs() {
  if (N.metadata.value != null) return !0;
  let e = globalThis.Server;
  if (hs(e))
    ol(e);
  else {
    const t = typeof localStorage < "u" ? localStorage.getItem(Wt) : null;
    if (t)
      try {
        ol(JSON.parse(t));
      } catch {
        console.error(`Could not JSON.parse ${Wt} from localStorage`);
      }
  }
  return N.metadata.value != null;
}
async function Ws(e, t) {
  let l = t ? await t() : await fetch(e);
  if (l.ok) {
    let n = await l.text();
    ol(JSON.parse(n));
  } else
    console.error(`Could not download ${t ? "AppMetadata" : e}: ${l.statusText}`);
  hs(N.metadata.value) || console.warn("AppMetadata is not available");
}
async function Ja(e) {
  const { olderThan: t, resolvePath: l, resolve: n } = e || {};
  let o = bs() && t !== 0;
  if (o && t) {
    let r = gt(N.metadata.value?.date);
    (!r || (/* @__PURE__ */ new Date()).getTime() - r.getTime() > t) && (o = !1);
  }
  if (!o) {
    const r = e.client ?? Ne("client");
    await Xa({ client: r, resolvePath: l, resolve: n });
  }
  return N.metadata.value;
}
async function Xa(e) {
  const { client: t, resolvePath: l, resolve: n } = e;
  if (!((l || n) && (await Ws(l || Wt, n), N.metadata.value != null))) {
    if (t != null) {
      const o = await t.api(new Ra());
      o.succeeded && ol(o.response);
    }
    if (N.metadata.value == null)
      return await Ws(Wt), N.metadata.value;
  }
}
function Je(e, t) {
  if (N.config.typeResolver) {
    let u = N.config.typeResolver(e, t);
    if (u) return u;
  }
  let l = N.metadata.value?.api;
  if (!l || !e) return null;
  let n = l.types.find((u) => u.name.toLowerCase() === e.toLowerCase() && (!t || u.namespace == t));
  if (n) return n;
  let o = It(e);
  if (o) return o.request;
  let r = l.operations.find((u) => u.response && u.response.name.toLowerCase() === e.toLowerCase() && (!t || u.response.namespace == t));
  return r ? r.response : null;
}
function It(e) {
  if (N.config.apiResolver) {
    const n = N.config.apiResolver(e);
    if (n) return n;
  }
  let t = N.metadata.value?.api;
  return t ? t.operations.find((n) => n.request.name.toLowerCase() === e.toLowerCase()) : null;
}
function Ya({ dataModel: e }) {
  const t = N.metadata.value?.api;
  if (!t) return [];
  let l = t.operations;
  if (e) {
    const n = typeof e == "string" ? Je(e) : e;
    l = l.filter((o) => wn(o.dataModel, n));
  }
  return l;
}
function ws(e) {
  return e ? Je(e.name, e.namespace) : null;
}
function wn(e, t) {
  return e && t && e.name === t.name && (!e.namespace || !t.namespace || e.namespace === t.namespace);
}
function eo(e, t) {
  let l = Je(e);
  return l && l.properties && l.properties.find((o) => o.name.toLowerCase() === t.toLowerCase());
}
function xn(e) {
  return kn(Je(e));
}
function kn(e) {
  if (e && e.isEnum && e.enumNames != null) {
    let t = {};
    for (let l = 0; l < e.enumNames.length; l++) {
      const n = (e.enumDescriptions ? e.enumDescriptions[l] : null) || e.enumNames[l], o = (e.enumValues != null ? e.enumValues[l] : null) || e.enumNames[l];
      t[o] = n;
    }
    return t;
  }
  return null;
}
function Cn(e) {
  if (!e) return null;
  let t = {}, l = e.input && e.input.allowableEntries;
  if (l) {
    for (let o = 0; o < l.length; o++) {
      let r = l[o];
      t[r.key] = r.value;
    }
    return t;
  }
  let n = e.allowableValues || (e.input ? e.input.allowableValues : null);
  if (n) {
    for (let o = 0; o < n.length; o++) {
      let r = n[o];
      t[r] = r;
    }
    return t;
  }
  if (e.isEnum) {
    const o = e.genericArgs && e.genericArgs.length == 1 ? e.genericArgs[0] : e.type, r = Je(o);
    if (r)
      return kn(r);
  }
  return null;
}
function xs(e) {
  if (!e) return;
  const t = [];
  return Object.keys(e).forEach((l) => t.push({ key: l, value: e[l] })), t;
}
function to(e, t) {
  const n = ((o, r) => Object.assign({
    id: o,
    name: o,
    type: r
  }, t))(e.name, t?.type || qa(e) || "text");
  return e.isEnum && (n.type = "select", n.allowableEntries = xs(Cn(e))), n;
}
function lo(e) {
  let t = [];
  if (e) {
    const l = et(e), n = It(e.name), o = ws(n?.dataModel);
    l.forEach((r) => {
      if (!bn(r)) return;
      const u = to(r, r.input);
      if (u.id = ya(u.id), u.type == "file" && r.uploadTo && !u.accept) {
        const d = N.metadata.value?.plugins.filesUpload?.locations.find((f) => f.name == r.uploadTo);
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
            $dataModelFields: o ? et(o).map((v) => v.name) : [],
            ...N.config.scopeWhitelist
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
function ks(e, t) {
  if (!t.type)
    return console.error("enumDescriptions missing {type:'EnumType'} options"), [`${e}`];
  const l = Je(t.type);
  if (!l?.enumValues)
    return console.error(`Could not find metadata for ${t.type}`), [`${e}`];
  const n = [];
  for (let o = 0; o < l.enumValues.length; o++) {
    const r = parseInt(l.enumValues[o]);
    r > 0 && (r & e) === r && n.push(l.enumDescriptions?.[o] || l.enumNames?.[o] || `${e}`);
  }
  return n;
}
function $n(e) {
  return (t) => typeof t == "number" ? ks(t, { type: e }) : t;
}
function et(e) {
  if (!e) return [];
  let t = [], l = {};
  function n(o) {
    o.forEach((r) => {
      l[r.name] || (l[r.name] = 1, t.push(r));
    });
  }
  for (; e; )
    e.properties && n(e.properties), e = e.inherits ? ws(e.inherits) : null;
  return t.map((o) => o.type.endsWith("[]") ? { ...o, type: "List`1", genericArgs: [o.type.substring(0, o.type.length - 2)] } : o);
}
function yl(e, t) {
  return e.request.implements?.some((l) => l.name === t) || !1;
}
function Jt(e) {
  return e ? Ln(e, et(e)) : null;
}
function Ln(e, t) {
  let l = t.find((r) => r.name.toLowerCase() === "id");
  if (l && l.isPrimaryKey) return l;
  let o = t.find((r) => r.isPrimaryKey) || l;
  if (!o) {
    let r = Re.model(e);
    if (r)
      return Ee(Je(r), (u) => Jt(u));
    console.error(`Primary Key not found in ${e.name}`);
  }
  return o || null;
}
function so(e, t) {
  return Ee(Jt(e), (l) => ye(t, l.name));
}
function Vn(e, t, l) {
  return e && e.valueType === "none" ? "" : l.key === "%In" || l.key === "%Between" ? `(${l.value})` : no(t, l.value);
}
function no(e, t) {
  return e ? (e = pn(e), ts(e) || e === "Boolean" ? t : Ka(e) ? `[${t}]` : `'${t}'`) : t;
}
function ht(e, t) {
  return { name: e, value: t };
}
const Mn = [
  ht("=", "%"),
  ht("!=", "%!"),
  ht(">=", ">%"),
  ht(">", "%>"),
  ht("<=", "%<"),
  ht("<", "<%"),
  ht("In", "%In"),
  ht("Between", "%Between"),
  { name: "Starts With", value: "%StartsWith", types: "string" },
  { name: "Contains", value: "%Contains", types: "string" },
  { name: "Ends With", value: "%EndsWith", types: "string" },
  { name: "Exists", value: "%IsNotNull", valueType: "none" },
  { name: "Not Exists", value: "%IsNull", valueType: "none" }
];
function vt() {
  const e = c(() => N.metadata.value?.app || null), t = c(() => N.metadata.value?.api || null), l = c(() => N.metadata.value?.plugins?.autoQuery?.viewerConventions || Mn);
  return bs(), {
    loadMetadata: Ja,
    getMetadata: Wa,
    setMetadata: ol,
    clearMetadata: Ga,
    metadataApp: e,
    metadataApi: t,
    filterDefinitions: l,
    typeOf: Je,
    typeOfRef: ws,
    typeEquals: wn,
    apiOf: It,
    findApis: Ya,
    typeName: Ua,
    typeName2: gn,
    property: eo,
    enumOptions: xn,
    propertyOptions: Cn,
    createFormLayout: lo,
    typeProperties: et,
    supportsProp: bn,
    Crud: Re,
    Apis: bt,
    getPrimaryKey: Jt,
    getPrimaryKeyByProps: Ln,
    getId: so,
    createDto: al,
    makeDto: _a,
    toFormValues: sl,
    formValues: Za,
    isComplexProp: hn,
    asKvps: xs,
    expandEnumFlags: ks,
    enumFlagsConverter: $n
  };
}
class We {
  static Lookup = {};
  static async getOrFetchValue(t, l, n, o, r, u, d) {
    const f = We.getValue(n, d, r);
    return f ?? (await We.fetchLookupIds(t, l, n, o, r, u, [d]), We.getValue(n, d, r));
  }
  static getValue(t, l, n) {
    const o = We.Lookup[t];
    if (o) {
      const r = o[l];
      if (r)
        return n = n.toLowerCase(), r[n];
    }
  }
  static setValue(t, l, n, o) {
    const r = We.Lookup[t] ?? (We.Lookup[t] = {}), u = r[l] ?? (r[l] = {});
    n = n.toLowerCase(), u[n] = o;
  }
  static setRefValue(t, l) {
    const n = ye(l, t.refId);
    if (n == null || t.refLabel == null)
      return null;
    const o = ye(l, t.refLabel);
    return We.setValue(t.model, n, t.refLabel, o), o;
  }
  static async fetchLookupIds(t, l, n, o, r, u, d) {
    const f = l.operations.find((v) => Re.isAnyQuery(v) && v.dataModel?.name == n);
    if (f) {
      const v = We.Lookup[n] ?? (We.Lookup[n] = {}), p = [];
      Object.keys(v).forEach((V) => {
        const M = v[V];
        ye(M, r) && p.push(V);
      });
      const $ = d.filter((V) => !p.includes(V));
      if ($.length == 0)
        return;
      const b = u ? null : `${o},${r}`, y = {
        [o + "In"]: $.join(",")
      };
      b && (y.fields = b);
      const h = al(f, y), k = await t.api(h, { jsconfig: "edv,eccn" });
      if (k.succeeded)
        (ye(k.response, "results") || []).forEach((M) => {
          if (!ye(M, o)) {
            console.error(`result[${o}] == null`, M);
            return;
          }
          const Y = `${ye(M, o)}`, E = ye(M, r);
          r = r.toLowerCase();
          const D = v[Y] ?? (v[Y] = {});
          D[r] = `${E}`;
        });
      else {
        console.error(`Failed to call ${f.request.name}`);
        return;
      }
    }
  }
}
let ls = () => (/* @__PURE__ */ new Date()).getTime(), ao = ["/", "T", ":", "-"], dt = {
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
}, oo = new Intl.RelativeTimeFormat(dt.locale, {}), Gs = 1440 * 60 * 1e3 * 365, Kl = {
  year: Gs,
  month: Gs / 12,
  day: 1440 * 60 * 1e3,
  hour: 3600 * 1e3,
  minute: 60 * 1e3,
  second: 1e3
}, Ft = {
  currency: Tn,
  bytes: Fn,
  link: An,
  linkTel: In,
  linkMailTo: Pn,
  icon: Dn,
  iconRounded: Bn,
  attachment: jn,
  hidden: On,
  time: Hn,
  relativeTime: $s,
  relativeTimeFromMs: Dl,
  enumFlags: En,
  formatDate: Xt,
  formatNumber: Cs
};
"iconOnError" in globalThis || (globalThis.iconOnError = Il);
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
    typeof e[t] == "function" && (Ft[t] = e[t]);
  });
}
function Sn() {
  return Ft;
}
function cl(e, t) {
  return t ? ct("span", e, t) : e;
}
function Tn(e, t) {
  const l = ft(t, ["currency"]);
  return cl(new Intl.NumberFormat(void 0, { style: "currency", currency: t?.currency || "USD" }).format(e), l);
}
function Fn(e, t) {
  return cl(gs(e), t);
}
function An(e, t) {
  return ct("a", e, Ol({ ...t, href: e }));
}
function In(e, t) {
  return ct("a", e, Ol({ ...t, href: `tel:${e}` }));
}
function Pn(e, t) {
  t || (t = {});
  let { subject: l, body: n } = t, o = ft(t, ["subject", "body"]), r = {};
  return l && (r.subject = l), n && (r.body = n), ct("a", e, Ol({ ...o, href: `mailto:${Zt(e, r)}` }));
}
function Dn(e, t) {
  return ct("img", void 0, Object.assign({ class: "w-6 h-6", title: e, src: Kt(e), onerror: "iconOnError(this)" }, t));
}
function Bn(e, t) {
  return ct("img", void 0, Object.assign({ class: "w-8 h-8 rounded-full", title: e, src: Kt(e), onerror: "iconOnError(this)" }, t));
}
function jn(e, t) {
  let l = fs(e), o = dl(l) == null || ms(e) ? Kt(e) : ps(e);
  const r = Kt(o);
  let u = t && (t["icon-class"] || t.iconClass), d = ct("img", void 0, Object.assign({ class: "w-6 h-6", src: r, onerror: "iconOnError(this,'att')" }, u ? { class: u } : null)), f = `<span class="pl-1">${l}</span>`;
  return ct("a", d + f, Object.assign({ class: "flex", href: Kt(e), title: e }, t ? ft(t, ["icon-class", "iconClass"]) : null));
}
function On(e) {
  return "";
}
function Hn(e, t) {
  let l = typeof e == "string" ? new Date(nn(e) * 1e3) : Al(e) ? gt(e) : null;
  return cl(l ? xa(l) : e, t);
}
function Xt(e, t) {
  if (e == null) return "";
  let l = typeof e == "number" ? new Date(e) : typeof e == "string" ? gt(e) : e;
  if (!Al(l))
    return console.warn(`${l} is not a Date value`), e == null ? "" : `${e}`;
  let n = dt.date ? Bl(dt.date) : null;
  return cl(typeof n == "function" ? n(l) : wa(l), t);
}
function Cs(e, t) {
  if (typeof e != "number") return e;
  let l = dt.number ? Bl(dt.number) : null, n = typeof l == "function" ? l(e) : `${e}`;
  return n === "" && (console.warn(`formatNumber(${e}) => ${n}`, l), n = `${e}`), cl(n, t);
}
function xl(e) {
  const t = Math.floor(e / 1e3), l = Math.floor(t / 60), n = Math.floor(l / 60), o = Math.floor(n / 24);
  return o > 0 ? `${o}d ${xl(e - o * 24 * 60 * 6e4)}` : n > 0 ? `${n}h ${xl(e - n * 60 * 6e4)}` : l > 0 ? `${l}m ${xl(e - l * 6e4)}` : t > 0 ? `${t}s` : `${e}ms`;
}
function co(e) {
  return e >= 1e9 ? (e / 1e9).toFixed(1) + "b" : e >= 1e6 ? (e / 1e6).toFixed(1) + "m" : e >= 1e3 ? (e / 1e3).toFixed(1) + "k" : e.toLocaleString();
}
function Rn(e, t, l) {
  let n = Ca(e), o = t ? Bl(t) : null;
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
  let r = n != null ? Al(n) ? Xt(n, l) : typeof n == "number" ? Cs(n, l) : n : null;
  return r ?? "";
}
function il(e, t, l) {
  return Bt(e) ? Rn(e, t, l) : po(e, t, l);
}
function fo(e) {
  if (e == null) return NaN;
  if (typeof e == "number")
    return e;
  if (Al(e))
    return e.getTime() - ls();
  if (typeof e == "string") {
    let t = Number(e);
    if (!isNaN(t))
      return t;
    if (e[0] === "P" || e.startsWith("-P"))
      return nn(e) * 1e3 * -1;
    if (ka(e, ao) >= 0)
      return gt(e).getTime() - ls();
  }
  return NaN;
}
function Dl(e, t) {
  for (let l in Kl)
    if (Math.abs(e) > Kl[l] || l === "second")
      return (t || oo).format(Math.round(e / Kl[l]), l);
}
function $s(e, t) {
  let l = fo(e);
  return isNaN(l) ? "" : Dl(l, t);
}
function vo(e, t) {
  return Dl(e.getTime() - (t ? t.getTime() : ls()));
}
function En(e, t) {
  return ks(e, t).join(", ");
}
function Bl(e) {
  if (!e) return null;
  let { method: t, options: l } = e, n = `${t}(${l})`, o = Ft[n] || Ft[t];
  if (typeof o == "function") return o;
  let r = e.locale || dt.locale;
  if (t.startsWith("Intl.")) {
    let u = r ? `'${r}'` : "undefined", d = `return new ${t}(${u},${l || "undefined"})`;
    try {
      let f = Function(d)();
      return o = t === "Intl.DateTimeFormat" ? (v) => f.format(gt(v)) : t === "Intl.NumberFormat" ? (v) => f.format(Number(v)) : t === "Intl.RelativeTimeFormat" ? (v) => $s(v, f) : (v) => f.format(v), Ft[n] = o;
    } catch (f) {
      console.error(`Invalid format: ${d}`, f);
    }
  } else {
    let u = globalThis[t];
    if (typeof u == "function") {
      let d = l != null ? Function("return " + l)() : void 0;
      return o = (f) => u(f, d, r), Ft[n] = o;
    }
    console.error(`No '${t}' function exists`, Object.keys(Ft));
  }
  return null;
}
function Nn(e, t) {
  return e ? e.length > t ? e.substring(0, t) + "..." : e : "";
}
function zn(e) {
  return e.substring(0, 6) === "/Date(" ? Xt(gt(e)) : e;
}
function mo(e) {
  return Ls(Gt(e)).replace(/"/g, "");
}
function Un(e) {
  if (e == null || e === "") return "";
  if (typeof e == "string")
    try {
      return JSON.parse(e);
    } catch {
      console.warn("couldn't parse as JSON", e);
    }
  return e;
}
function Ls(e, t = 4) {
  return e = Un(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : JSON.stringify(e, void 0, t);
}
function go(e) {
  return e = Un(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : (e = Object.assign({}, e), e = Gt(e), Ls(e));
}
function Gt(e) {
  if (e == null) return null;
  if (typeof e == "string") return zn(e);
  if (Bt(e)) return e;
  if (e instanceof Date) return Xt(e);
  if (Array.isArray(e)) return e.map(Gt);
  if (typeof e == "object") {
    let t = {};
    return Object.keys(e).forEach((l) => {
      l != "__type" && (t[l] = Gt(e[l]));
    }), t;
  }
  return e;
}
function po(e, t, l) {
  let n = e;
  if (Array.isArray(e)) {
    if (Bt(e[0]))
      return n.join(",");
    e[0] != null && (n = e[0]);
  }
  if (n == null) return "";
  if (n instanceof Date) return Xt(n, l);
  let o = Object.keys(n), r = [];
  for (let u = 0; u < Math.min(dt.maxNestedFields, o.length); u++) {
    let d = o[u], f = `${Gt(n[d])}`;
    r.push(`<b class="font-medium">${d}</b>: ${Wl(Nn(zn(f), dt.maxNestedFieldLength))}`);
  }
  return o.length > 2 && r.push("..."), ct("span", "{ " + r.join(", ") + " }", Object.assign({ title: Wl(mo(e)) }, l));
}
function Bm() {
  return {
    Formats: io,
    setDefaultFormats: ro,
    getFormatters: Sn,
    setFormatters: uo,
    formatValue: il,
    formatter: Bl,
    dateInputFormat: jl,
    currency: Tn,
    bytes: Fn,
    link: An,
    linkTel: In,
    linkMailTo: Pn,
    icon: Dn,
    iconRounded: Bn,
    attachment: jn,
    hidden: On,
    time: Hn,
    relativeTime: $s,
    relativeTimeFromDate: vo,
    relativeTimeFromMs: Dl,
    enumFlags: En,
    formatDate: Xt,
    formatNumber: Cs,
    humanifyMs: xl,
    humanifyNumber: co,
    indentJson: Ls,
    prettyJson: go,
    scrub: Gt,
    truncate: Nn,
    apiValueFmt: Rn,
    iconOnError: Il
  };
}
const yo = ["title"], ho = /* @__PURE__ */ ue({
  __name: "RouterLink",
  props: {
    to: {}
  },
  setup(e) {
    const t = e, { config: l } = Ot(), n = () => l.value.navigate(t.to ?? "/");
    return (o, r) => (a(), i("a", Ce({
      onClick: He(n, ["prevent"]),
      title: e.to,
      href: "javascript:void(0)"
    }, o.$attrs), [
      O(o.$slots, "default")
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
    const n = this.callbacks[t];
    typeof n == "function" && n(t, l);
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
class N {
  static config = {
    redirectSignIn: "/signin",
    redirectSignOut: "/auth/logout",
    navigate: (t) => location.href = t,
    assetsPathResolver: (t) => t,
    fallbackPathResolver: (t) => t,
    storage: new wo(),
    tableIcon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><g fill='none' stroke='currentColor' stroke-width='1.5'><path d='M5 12v6s0 3 7 3s7-3 7-3v-6'/><path d='M5 6v6s0 3 7 3s7-3 7-3V6'/><path d='M12 3c7 0 7 3 7 3s0 3-7 3s-7-3-7-3s0-3 7-3Z'/></g></svg>" },
    scopeWhitelist: {
      enumFlagsConverter: $n,
      ...Sn()
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
  static user = A(null);
  static metadata = Zl(null);
  static components = {
    RouterLink: ho
  };
  static component(t) {
    const l = N.components[t];
    if (l) return l;
    const n = Qs(t), o = Object.keys(N.components).find((r) => Qs(r) === n);
    return o && N.components[o] || null;
  }
  static interceptors = new bo();
}
function xo(e) {
  N.config = Object.assign(N.config, e);
}
function ko(e) {
  N.autoQueryGridDefaults = Object.assign(N.autoQueryGridDefaults, e);
}
function Vs(e) {
  return e && N.config.assetsPathResolver ? N.config.assetsPathResolver(e) : e;
}
function Co(e) {
  return e && N.config.fallbackPathResolver ? N.config.fallbackPathResolver(e) : e;
}
function $o(e, t) {
  N.interceptors.register(e, t);
}
function Ot() {
  const e = c(() => N.config), t = c(() => N.autoQueryGridDefaults), l = N.events;
  return {
    Sole: N,
    config: e,
    setConfig: xo,
    events: l,
    autoQueryGridDefaults: t,
    setAutoQueryGridDefaults: ko,
    assetsPathResolver: Vs,
    fallbackPathResolver: Co,
    registerInterceptor: $o
  };
}
function jl(e) {
  if (e == null || typeof e == "object") return "";
  const t = gt(e);
  return t == null || t.toString() == "Invalid Date" ? "" : t.toISOString().substring(0, 10) ?? "";
}
function qn(e) {
  if (e == null || typeof e == "object") return "";
  const t = gt(e);
  return t == null || t.toString() == "Invalid Date" ? "" : t.toISOString().substring(0, 19) ?? "";
}
function Qn(e) {
  return e == null ? "" : Ma(e);
}
function ss(e, t) {
  return N.config.inputValue ? N.config.inputValue(e, t) : e === "date" ? jl(t) : e === "datetime-local" ? qn(t) : e === "time" ? Qn(t) : e === "number" || e === "range" ? t == null ? "" : Number(t) : t;
}
function Kn(e, t) {
  e.value = null, wt(() => e.value = t);
}
function qt(e) {
  return Object.keys(e).forEach((t) => {
    const l = e[t];
    e[t] = en(l) ? G(l) : l;
  }), e;
}
function xt(e, t, l) {
  l ? (t.value = e.entering.cls + " " + e.entering.from, setTimeout(() => t.value = e.entering.cls + " " + e.entering.to, 0)) : (t.value = e.leaving.cls + " " + e.leaving.from, setTimeout(() => t.value = e.leaving.cls + " " + e.leaving.to, 0));
}
function kl(e) {
  if (typeof document > "u") return;
  let t = e?.after || document.activeElement, l = t && t.form;
  if (l) {
    let n = ':not([disabled]):not([tabindex="-1"])', o = l.querySelectorAll(`a:not([disabled]), button${n}, input[type=text]${n}, [tabindex]${n}`), r = Array.prototype.filter.call(
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
  let n = l.cls || l.className || l.class;
  return n && (l = ft(l, ["cls", "class", "className"]), l.class = n), t == null ? `<${e}` + ns(l) + "/>" : `<${e}` + ns(l) + `>${t || ""}</${e}>`;
}
function ns(e) {
  return Object.keys(e).reduce((t, l) => `${t} ${l}="${Wl(e[l])}"`, "");
}
function Ol(e) {
  return Object.assign({ target: "_blank", rel: "noopener", class: "text-blue-600" }, e);
}
function Kt(e) {
  return Vs(e);
}
let Lo = ["string", "number", "boolean", "null", "undefined"];
function Bt(e) {
  return Lo.indexOf(typeof e) >= 0 || e instanceof Date;
}
function rl(e) {
  return !Bt(e);
}
function Vl(e) {
  return typeof e == "string" ? JSON.parse(e) : null;
}
function Ms(e, t) {
  if (typeof history < "u") {
    const l = t ? Zt(an(location.href, "?"), e) : Va(location.href, e);
    history.pushState({}, "", l);
  }
}
function Hl(e, t) {
  if (["function", "Function", "eval", "=>", ";"].some((o) => e.includes(o)))
    throw new Error(`Unsafe script: '${e}'`);
  const n = Object.assign(
    Object.keys(globalThis).reduce((o, r) => (o[r] = void 0, o), {}),
    t
  );
  return new Function("with(this) { return (" + e + ") }").call(n);
}
function as(e) {
  typeof navigator < "u" && navigator.clipboard.writeText(e);
}
function Ss(e) {
  const t = N.config.storage.getItem(e);
  return t ? JSON.parse(t) : null;
}
function Rl(e, t) {
  return Zt(`swr.${La(e)}`, t ? Object.assign({}, e, t) : e);
}
function Vo(e) {
  if (e.request) {
    const t = Rl(e.request, e.args);
    N.config.storage.removeItem(t);
  }
}
async function _n(e, t, l, n, o) {
  const r = Rl(t, n);
  l(new Ge({ response: Ss(r) }));
  const u = await e.api(t, n, o);
  if (u.succeeded && u.response) {
    u.response._date = (/* @__PURE__ */ new Date()).valueOf();
    const d = JSON.stringify(u.response);
    N.config.storage.setItem(r, d), l(u);
  }
  return u;
}
function Zn(e, t) {
  let l = null;
  return (...n) => {
    l && clearTimeout(l), l = setTimeout(() => {
      e(...n);
    }, t || 100);
  };
}
function _t(e) {
  return typeof e == "string" ? e.split(",") : e || [];
}
function At(e, t) {
  const l = _t(t);
  return e.reduce((n, o) => (n[o] = !l.includes(o), n), {});
}
function Mo(e) {
  return new Promise((t) => setTimeout(t, e));
}
function Wn(e) {
  const t = [], l = [];
  for (const n of e) {
    const o = n.toLowerCase();
    l.includes(o) || (t.push(n), l.push(o));
  }
  return t;
}
function Gn() {
  return {
    dateInputFormat: jl,
    dateTimeInputFormat: qn,
    timeInputFormat: Qn,
    textInputValue: ss,
    setRef: Kn,
    unRefs: qt,
    transition: xt,
    focusNextElement: kl,
    getTypeName: Ht,
    htmlTag: ct,
    htmlAttrs: ns,
    linkAttrs: Ol,
    toAppUrl: Kt,
    isPrimitive: Bt,
    isComplexType: rl,
    pushState: Ms,
    scopedExpr: Hl,
    copyText: as,
    fromCache: Ss,
    swrCacheKey: Rl,
    swrClear: Vo,
    swrApi: _n,
    asStrings: _t,
    asOptions: At,
    createDebounce: Zn,
    delay: Mo,
    uniqueIgnoreCase: Wn
  };
}
function fl(e) {
  const t = A(!1), l = A(), n = A(), o = e ?? Ne("client");
  function r({ message: h, errorCode: k, fieldName: V, errors: M }) {
    return k || (k = "Exception"), M || (M = []), l.value = V ? new Ql({
      errorCode: k,
      message: h,
      errors: [new Ks({ fieldName: V, errorCode: k, message: h })]
    }) : new Ql({ errorCode: k, message: h, errors: M });
  }
  function u({ fieldName: h, message: k, errorCode: V }) {
    if (V || (V = "Exception"), !l.value)
      r({ fieldName: h, message: k, errorCode: V });
    else {
      let M = new Ql(l.value);
      M.errors = [
        ...(M.errors || []).filter((Y) => Y.fieldName?.toLowerCase() !== h?.toLowerCase()),
        new Ks({ fieldName: h, message: k, errorCode: V })
      ], l.value = M;
    }
  }
  async function d(h, k, V) {
    t.value = !0;
    let M = await o.api(qt(h), k, V);
    return t.value = !1, n.value = M.response, l.value = M.error, M;
  }
  async function f(h, k, V) {
    t.value = !0;
    let M = await o.apiVoid(qt(h), k, V);
    return t.value = !1, n.value = M.response, l.value = M.error, M;
  }
  async function v(h, k, V, M) {
    t.value = !0;
    let Y = await o.apiForm(qt(h), k, V, M);
    return t.value = !1, n.value = Y.response, l.value = Y.error, Y;
  }
  async function p(h, k, V, M) {
    t.value = !0;
    let Y = await o.apiFormVoid(qt(h), k, V, M);
    return t.value = !1, n.value = Y.response, l.value = Y.error, Y;
  }
  async function $(h, k, V, M) {
    return _n(o, h, k, V, M);
  }
  function b(h, k) {
    const V = A(new Ge()), M = Zn(async (Y) => {
      V.value = await o.api(Y);
    }, k?.delayMs);
    return Cl(async () => {
      const Y = h(), E = Ss(Rl(Y));
      E && (V.value = new Ge({ response: E })), k?.delayMs === 0 ? V.value = await o.api(Y) : M(Y);
    }), (async () => V.value = await o.api(h(), k?.args, k?.method))(), V;
  }
  let y = { setError: r, addFieldError: u, loading: t, error: l, api: d, apiVoid: f, apiForm: v, apiFormVoid: p, swr: $, swrEffect: b, unRefs: qt, setRef: Kn };
  return ul("ApiState", y), y;
}
function Jn(e) {
  return e && e.SessionId ? Sa(e) : e;
}
function So(e) {
  N.user.value = Jn(e), N.events.publish("signIn", e);
}
function To() {
  N.user.value = null, N.events.publish("signOut", null);
}
const Ts = (e) => e?.roles || [], Fs = (e) => e?.permissions || [];
function Xn(e) {
  return Ts(N.user.value).indexOf(e) >= 0;
}
function Fo(e) {
  return Fs(N.user.value).indexOf(e) >= 0;
}
function As() {
  return Xn("Admin");
}
function nl(e) {
  if (!e) return !1;
  if (!e.requiresAuth)
    return !0;
  const t = N.user.value;
  if (!t)
    return !1;
  if (As())
    return !0;
  let [l, n] = [Ts(t), Fs(t)], [o, r, u, d] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ];
  return !(!o.every((f) => l.indexOf(f) >= 0) || u.length > 0 && !u.some((f) => l.indexOf(f) >= 0) || !r.every((f) => n.indexOf(f) >= 0) || d.length > 0 && !d.every((f) => n.indexOf(f) >= 0));
}
function Ao(e) {
  if (!e || !e.requiresAuth) return null;
  const t = N.user.value;
  if (!t)
    return `<b>${e.request.name}</b> requires Authentication`;
  if (As())
    return null;
  let [l, n] = [Ts(t), Fs(t)], [o, r, u, d] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ], f = o.filter((p) => l.indexOf(p) < 0);
  if (f.length > 0)
    return `Requires ${f.map((p) => "<b>" + p + "</b>").join(", ")} Role` + (f.length > 1 ? "s" : "");
  let v = r.filter((p) => n.indexOf(p) < 0);
  return v.length > 0 ? `Requires ${v.map((p) => "<b>" + p + "</b>").join(", ")} Permission` + (v.length > 1 ? "s" : "") : u.length > 0 && !u.some((p) => l.indexOf(p) >= 0) ? `Requires any ${u.filter((p) => l.indexOf(p) < 0).map((p) => "<b>" + p + "</b>").join(", ")} Role` + (f.length > 1 ? "s" : "") : d.length > 0 && !d.every((p) => n.indexOf(p) >= 0) ? `Requires any ${d.filter((p) => n.indexOf(p) < 0).map((p) => "<b>" + p + "</b>").join(", ")} Permission` + (v.length > 1 ? "s" : "") : null;
}
function Is() {
  const e = c(() => N.user.value || null), t = c(() => N.user.value != null);
  return { signIn: So, signOut: To, user: e, toAuth: Jn, isAuthenticated: t, hasRole: Xn, hasPermission: Fo, isAdmin: As, canAccess: nl, invalidAccessMessage: Ao };
}
function Ze(e, t) {
  return Array.isArray(e) ? e.indexOf(t) >= 0 : e == t || e.includes(t);
}
const Ml = {
  blue: "text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-200",
  purple: "text-purple-600 dark:text-purple-400 hover:text-purple-800 dark:hover:text-purple-200",
  red: "text-red-700 dark:text-red-400 hover:text-red-900 dark:hover:text-red-200",
  green: "text-green-600 dark:text-green-400 hover:text-green-800 dark:hover:text-green-200",
  sky: "text-sky-600 dark:text-sky-400 hover:text-sky-800 dark:hover:text-sky-200",
  cyan: "text-cyan-600 dark:text-cyan-400 hover:text-cyan-800 dark:hover:text-cyan-200",
  indigo: "text-indigo-600 dark:text-indigo-400 hover:text-indigo-800 dark:hover:text-indigo-200"
}, rt = {
  base: "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 disabled:bg-slate-50 dark:disabled:bg-slate-900 disabled:text-slate-500 disabled:border-slate-200 dark:disabled:border-slate-700 disabled:shadow-none",
  invalid: "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500",
  valid: "shadow-sm focus:ring-indigo-500 focus:border-indigo-500 border-gray-300 dark:border-gray-600"
}, tl = {
  panelClass: "shadow sm:rounded-md",
  formClass: "space-y-6 bg-white dark:bg-black py-6 px-4 sm:p-6",
  headingClass: "text-lg font-medium leading-6 text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400"
}, Qt = {
  panelClass: "pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg",
  formClass: "flex h-full flex-col divide-y divide-gray-200 dark:divide-gray-700 shadow-xl bg-white dark:bg-black",
  titlebarClass: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6",
  headingClass: "text-lg font-medium text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400",
  closeButtonClass: "rounded-md bg-gray-50 dark:bg-gray-900 text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
}, os = {
  modalClass: "relative transform overflow-hidden rounded-lg bg-white dark:bg-black text-left shadow-xl transition-all sm:my-8",
  sizeClass: "sm:max-w-prose lg:max-w-screen-md xl:max-w-screen-lg 2xl:max-w-screen-xl sm:w-full"
}, Oe = {
  panelClass(e = "slideOver") {
    return e == "card" ? tl.panelClass : Qt.panelClass;
  },
  formClass(e = "slideOver") {
    return e == "card" ? tl.formClass : Qt.formClass;
  },
  headingClass(e = "slideOver") {
    return e == "card" ? tl.headingClass : Qt.headingClass;
  },
  subHeadingClass(e = "slideOver") {
    return e == "card" ? tl.subHeadingClass : Qt.subHeadingClass;
  },
  buttonsClass: "px-4 py-3 bg-gray-50 dark:bg-gray-900 sm:px-6 flex flex-wrap justify-between",
  legendClass: "text-base font-medium text-gray-900 dark:text-gray-100 text-center mb-4"
}, ge = {
  getGridClass(e = "stripedRows") {
    return ge.gridClass;
  },
  getGrid2Class(e = "stripedRows") {
    return Ze(e, "fullWidth") ? "overflow-x-auto" : ge.grid2Class;
  },
  getGrid3Class(e = "stripedRows") {
    return Ze(e, "fullWidth") ? "inline-block min-w-full py-2 align-middle" : ge.grid3Class;
  },
  getGrid4Class(e = "stripedRows") {
    return Ze(e, "whiteBackground") ? "" : Ze(e, "fullWidth") ? "overflow-hidden shadow-sm ring-1 ring-black/5" : ge.grid4Class;
  },
  getTableClass(e = "stripedRows") {
    return Ze(e, "fullWidth") || Ze(e, "verticalLines") ? "min-w-full divide-y divide-gray-300" : ge.tableClass;
  },
  getTheadClass(e = "stripedRows") {
    return Ze(e, "whiteBackground") ? "" : ge.theadClass;
  },
  getTheadRowClass(e = "stripedRows") {
    return ge.theadRowClass + (Ze(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
  },
  getTheadCellClass(e = "stripedRows") {
    return ge.theadCellClass + (Ze(e, "uppercaseHeadings") ? " uppercase" : "");
  },
  getTbodyClass(e = "stripedRows") {
    return (Ze(e, "whiteBackground") || Ze(e, "verticalLines") ? "divide-y divide-gray-200 dark:divide-gray-800" : ge.tableClass) + (Ze(e, "verticalLines") ? " bg-white" : "");
  },
  getTableRowClass(e = "stripedRows", t, l, n) {
    return (n ? "cursor-pointer " : "") + (l ? "bg-indigo-100 dark:bg-blue-800" : (n ? "hover:bg-yellow-50 dark:hover:bg-blue-900 " : "") + (Ze(e, "stripedRows") ? t % 2 == 0 ? "bg-white dark:bg-black" : "bg-gray-50 dark:bg-gray-800" : "bg-white dark:bg-black")) + (Ze(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
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
function Ct(e, t, l) {
  const n = e.filter((o) => o).join(" ");
  return l ??= N.config.filterInputClass == null ? void 0 : (o) => N.config.filterInputClass(o, t), l ? l(n) : n;
}
const jm = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  a: Ml,
  card: tl,
  dummy: Io,
  filterClass: Ct,
  form: Oe,
  grid: ge,
  input: rt,
  modal: os,
  slideOver: Qt
}, Symbol.toStringTag, { value: "Module" })), Po = { class: "flex items-center" }, Do = {
  key: 0,
  class: "flex-shrink-0 mr-3"
}, Bo = {
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
}, Oo = {
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
}, Ro = /* @__PURE__ */ ue({
  __name: "Alert",
  props: {
    type: { default: "warn" },
    hideIcon: { type: Boolean }
  },
  setup(e) {
    const t = e, l = c(() => t.type == "info" ? "bg-blue-50 dark:bg-blue-200" : t.type == "error" ? "bg-red-50 dark:bg-red-200" : t.type == "success" ? "bg-green-50 dark:bg-green-200" : "bg-yellow-50 dark:bg-yellow-200"), n = c(() => t.type == "info" ? "border-blue-400" : t.type == "error" ? "border-red-400" : t.type == "success" ? "border-green-400" : "border-yellow-400"), o = c(() => t.type == "info" ? "text-blue-700" : t.type == "error" ? "text-red-700" : t.type == "success" ? "text-green-700" : "text-yellow-700");
    return (r, u) => (a(), i("div", {
      class: g([l.value, n.value, "border-l-4 p-4"])
    }, [
      s("div", Po, [
        e.hideIcon ? x("", !0) : (a(), i("div", Do, [
          e.type == "warn" ? (a(), i("svg", Bo, [...u[0] || (u[0] = [
            s("path", {
              "fill-rule": "evenodd",
              d: "M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "error" ? (a(), i("svg", jo, [...u[1] || (u[1] = [
            s("path", {
              "fill-rule": "evenodd",
              d: "M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "info" ? (a(), i("svg", Oo, [...u[2] || (u[2] = [
            s("path", {
              "fill-rule": "evenodd",
              d: "M19 10.5a8.5 8.5 0 11-17 0 8.5 8.5 0 0117 0zM8.25 9.75A.75.75 0 019 9h.253a1.75 1.75 0 011.709 2.13l-.46 2.066a.25.25 0 00.245.304H11a.75.75 0 010 1.5h-.253a1.75 1.75 0 01-1.709-2.13l.46-2.066a.25.25 0 00-.245-.304H9a.75.75 0 01-.75-.75zM10 7a1 1 0 100-2 1 1 0 000 2z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : e.type == "success" ? (a(), i("svg", Ho, [...u[3] || (u[3] = [
            s("path", {
              "fill-rule": "evenodd",
              d: "M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z",
              "clip-rule": "evenodd"
            }, null, -1)
          ])])) : x("", !0)
        ])),
        s("div", null, [
          s("p", {
            class: g([o.value, "text-sm"])
          }, [
            O(r.$slots, "default")
          ], 2)
        ])
      ])
    ], 2));
  }
}), Eo = {
  key: 0,
  class: "rounded-md bg-green-50 dark:bg-green-200 p-4",
  role: "alert"
}, No = { class: "flex" }, zo = { class: "ml-3" }, Uo = { class: "text-sm font-medium text-green-800" }, qo = { key: 0 }, Qo = { class: "ml-auto pl-3" }, Ko = { class: "-mx-1.5 -my-1.5" }, _o = /* @__PURE__ */ ue({
  __name: "AlertSuccess",
  props: {
    message: {}
  },
  setup(e) {
    const t = A(!1);
    return (l, n) => t.value ? x("", !0) : (a(), i("div", Eo, [
      s("div", No, [
        n[2] || (n[2] = s("div", { class: "flex-shrink-0" }, [
          s("svg", {
            class: "h-5 w-5 text-green-400 dark:text-green-500",
            fill: "none",
            stroke: "currentColor",
            viewBox: "0 0 24 24",
            xmlns: "http://www.w3.org/2000/svg"
          }, [
            s("path", {
              "stroke-linecap": "round",
              "stroke-linejoin": "round",
              "stroke-width": "2",
              d: "M5 13l4 4L19 7"
            })
          ])
        ], -1)),
        s("div", zo, [
          s("h3", Uo, [
            e.message ? (a(), i("span", qo, T(e.message), 1)) : O(l.$slots, "default", { key: 1 })
          ])
        ]),
        s("div", Qo, [
          s("div", Ko, [
            s("button", {
              type: "button",
              class: "inline-flex rounded-md bg-green-50 dark:bg-green-200 p-1.5 text-green-500 dark:text-green-600 hover:bg-green-100 dark:hover:bg-green-800 dark:hover:text-white focus:outline-none focus:ring-2 focus:ring-green-600 dark:focus:ring-green-300 focus:ring-offset-2 focus:ring-offset-green-50 dark:ring-offset-green-900",
              onClick: n[0] || (n[0] = (o) => t.value = !0)
            }, [...n[1] || (n[1] = [
              s("span", { class: "sr-only" }, "Dismiss", -1),
              s("svg", {
                class: "h-5 w-5",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                s("path", { d: "M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" })
              ], -1)
            ])])
          ])
        ])
      ])
    ]));
  }
}), Zo = { class: "flex" }, Wo = { class: "ml-3" }, Go = { class: "text-sm text-red-700 dark:text-red-200" }, Jo = /* @__PURE__ */ ue({
  __name: "ErrorSummary",
  props: {
    status: {},
    except: {},
    class: {}
  },
  setup(e) {
    const t = e;
    let l = Ne("ApiState", void 0);
    const n = c(() => t.status || l?.error.value ? Ta.call({ responseStatus: t.status ?? l?.error.value }, t.except ?? []) : null);
    return (o, r) => n.value ? (a(), i("div", {
      key: 0,
      class: g(`bg-red-50 dark:bg-red-900 border-l-4 border-red-400 p-4 ${o.$props.class}`)
    }, [
      s("div", Zo, [
        r[0] || (r[0] = s("div", { class: "flex-shrink-0" }, [
          s("svg", {
            class: "h-5 w-5 text-red-400",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 24 24"
          }, [
            s("path", {
              fill: "currentColor",
              d: "M12 2c5.53 0 10 4.47 10 10s-4.47 10-10 10S2 17.53 2 12S6.47 2 12 2m3.59 5L12 10.59L8.41 7L7 8.41L10.59 12L7 15.59L8.41 17L12 13.41L15.59 17L17 15.59L13.41 12L17 8.41L15.59 7Z"
            })
          ])
        ], -1)),
        s("div", Wo, [
          s("p", Go, T(n.value), 1)
        ])
      ])
    ], 2)) : x("", !0);
  }
}), Xo = ["id", "aria-describedby"], Yo = /* @__PURE__ */ ue({
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
      s("div", null, T(e.description), 1)
    ], 8, Xo)) : x("", !0);
  }
}), Yn = ue({
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
        const { typeOf: r } = vt(), u = r(e.type);
        u || console.warn(`Type ${e.type} does not exist`), u?.icon ? l = u?.icon : console.warn(`Type ${e.type} does not have a [Svg] icon`);
      }
      let n = e.svg || l?.svg || "";
      if (n.startsWith("<svg ")) {
        let u = Fl(n, ">").indexOf("class="), d = `${l?.cls || ""} ${t.class || ""}`;
        if (u == -1)
          n = `<svg class="${d}" ${n.substring(4)}`;
        else {
          const f = u + 6 + 1;
          n = `${n.substring(0, f) + d} ${n.substring(f)}`;
        }
        return St("span", { innerHTML: n });
      } else
        return St("img", {
          class: [l?.cls, t.class],
          src: Vs(e.src || l?.uri),
          onError: (r) => Il(r.target)
        });
    };
  }
}), ei = { class: "text-2xl font-semibold text-gray-900 dark:text-gray-300" }, ti = { class: "flex" }, li = /* @__PURE__ */ ue({
  __name: "Loading",
  props: {
    imageClass: { default: "w-6 h-6" }
  },
  setup(e) {
    return (t, l) => (a(), i("div", ei, [
      s("div", ti, [
        (a(), i("svg", {
          class: g(["self-center inline mr-2 text-gray-200 animate-spin dark:text-gray-600 fill-gray-600 dark:fill-gray-300", e.imageClass]),
          role: "status",
          viewBox: "0 0 100 101",
          fill: "none",
          xmlns: "http://www.w3.org/2000/svg"
        }, [...l[0] || (l[0] = [
          s("path", {
            d: "M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z",
            fill: "currentColor"
          }, null, -1),
          s("path", {
            d: "M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z",
            fill: "currentFill"
          }, null, -1)
        ])], 2)),
        s("span", null, [
          O(t.$slots, "default")
        ])
      ])
    ]));
  }
}), si = ["href", "onClick"], ni = ["type"], Js = "inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 shadow-sm text-sm font-medium rounded-md text-gray-700 dark:text-gray-200 disabled:text-gray-400 bg-white dark:bg-black hover:bg-gray-50 hover:dark:bg-gray-900 disabled:hover:bg-white dark:disabled:hover:bg-black focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black", ai = /* @__PURE__ */ ue({
  __name: "OutlineButton",
  props: {
    type: { default: "submit" },
    href: {}
  },
  setup(e) {
    return (t, l) => {
      const n = U("router-link");
      return e.href ? (a(), X(n, {
        key: 0,
        to: e.href
      }, {
        default: we(({ navigate: o }) => [
          s("button", {
            class: g(Js),
            href: e.href,
            onClick: o
          }, [
            O(t.$slots, "default")
          ], 8, si)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", Ce({
        key: 1,
        type: e.type,
        class: Js
      }, t.$attrs), [
        O(t.$slots, "default")
      ], 16, ni));
    };
  }
}), oi = ["href", "onClick"], ii = ["type"], ri = /* @__PURE__ */ ue({
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
    }, n = c(() => "inline-flex justify-center rounded-md border border-transparent py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 dark:ring-offset-black text-white " + (l[t.color] || l.indigo));
    return (o, r) => {
      const u = U("router-link");
      return e.href ? (a(), X(u, {
        key: 0,
        to: e.href
      }, {
        default: we(({ navigate: d }) => [
          s("button", {
            class: g(n.value),
            href: e.href,
            onClick: d
          }, [
            O(o.$slots, "default")
          ], 10, oi)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", Ce({
        key: 1,
        type: e.type,
        class: n.value
      }, o.$attrs), [
        O(o.$slots, "default")
      ], 16, ii));
    };
  }
}), ui = ["type", "href", "onClick"], di = ["type"], Xs = "inline-flex justify-center rounded-md border border-gray-300 py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-400 dark:hover:text-white hover:bg-gray-50 dark:hover:bg-gray-700 focus:ring-indigo-500 dark:focus:ring-indigo-600 dark:ring-offset-black", ci = /* @__PURE__ */ ue({
  __name: "SecondaryButton",
  props: {
    type: {},
    href: {}
  },
  setup(e) {
    return (t, l) => {
      const n = U("router-link");
      return e.href ? (a(), X(n, {
        key: 0,
        to: e.href
      }, {
        default: we(({ navigate: o }) => [
          s("button", {
            type: e.type ?? "button",
            class: g(Xs),
            href: e.href,
            onClick: o
          }, [
            O(t.$slots, "default")
          ], 8, ui)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), i("button", Ce({
        key: 1,
        type: e.type ?? "button",
        class: Xs
      }, t.$attrs), [
        O(t.$slots, "default")
      ], 16, di));
    };
  }
}), fi = /* @__PURE__ */ ue({
  __name: "TextLink",
  props: {
    color: { default: "blue" }
  },
  setup(e) {
    const t = ca(), l = e, n = c(() => (Ml[l.color] || Ml.blue) + (t.href ? "" : " cursor-pointer"));
    return (o, r) => (a(), i("a", {
      class: g(n.value)
    }, [
      O(o.$slots, "default")
    ], 2));
  }
}), vi = {
  class: "flex",
  "aria-label": "Breadcrumb"
}, mi = {
  role: "list",
  class: "flex items-center space-x-4"
}, gi = ["href", "title"], pi = { class: "sr-only" }, yi = /* @__PURE__ */ ue({
  __name: "Breadcrumbs",
  props: {
    homeHref: { default: "/" },
    homeLabel: { default: "Home" }
  },
  setup(e) {
    return (t, l) => (a(), i("nav", vi, [
      s("ol", mi, [
        s("li", null, [
          s("div", null, [
            s("a", {
              href: e.homeHref,
              class: "text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400",
              title: e.homeLabel
            }, [
              l[0] || (l[0] = s("svg", {
                class: "h-6 w-6 flex-shrink-0",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                s("path", {
                  "fill-rule": "evenodd",
                  d: "M9.293 2.293a1 1 0 011.414 0l7 7A1 1 0 0117 11h-1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-3a1 1 0 00-1-1H9a1 1 0 00-1 1v3a1 1 0 01-1 1H5a1 1 0 01-1-1v-6H3a1 1 0 01-.707-1.707l7-7z",
                  "clip-rule": "evenodd"
                })
              ], -1)),
              s("span", pi, T(e.homeLabel), 1)
            ], 8, gi)
          ])
        ]),
        O(t.$slots, "default")
      ])
    ]));
  }
}), hi = { class: "flex items-center" }, bi = ["href", "title"], wi = ["title"], xi = /* @__PURE__ */ ue({
  __name: "Breadcrumb",
  props: {
    href: {},
    title: {}
  },
  setup(e) {
    return (t, l) => (a(), i("li", null, [
      s("div", hi, [
        l[0] || (l[0] = s("svg", {
          class: "h-6 w-6 flex-shrink-0 text-gray-400 dark:text-gray-500",
          xmlns: "http://www.w3.org/2000/svg",
          viewBox: "0 0 20 20",
          fill: "currentColor",
          "aria-hidden": "true"
        }, [
          s("path", {
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
          O(t.$slots, "default")
        ], 8, bi)) : (a(), i("span", {
          key: 1,
          class: "ml-4 text-lg font-medium text-gray-700 dark:text-gray-300",
          title: e.title
        }, [
          O(t.$slots, "default")
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
}, $i = /* @__PURE__ */ ue({
  __name: "NavList",
  props: {
    title: {}
  },
  setup(e) {
    return (t, l) => (a(), i("div", null, [
      e.title ? (a(), i("h2", ki, T(e.title), 1)) : x("", !0),
      s("ul", Ci, [
        O(t.$slots, "default")
      ])
    ]));
  }
}), Li = { class: "relative flex items-start space-x-4 py-6" }, Vi = { class: "flex-shrink-0" }, Mi = { class: "flex h-12 w-12 items-center justify-center rounded-lg bg-indigo-50 dark:bg-indigo-900" }, Si = { class: "min-w-0 flex-1" }, Ti = { class: "text-base font-medium text-gray-900 dark:text-gray-100" }, Fi = { class: "rounded-sm focus-within:ring-2 focus-within:ring-indigo-500 focus-within:ring-offset-2" }, Ai = ["href"], Ii = { class: "text-base text-gray-500" }, Pi = /* @__PURE__ */ ue({
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
      const n = U("Icon");
      return a(), i("li", Li, [
        s("div", Vi, [
          s("span", Mi, [
            pe(n, {
              class: "w-6 h-6 text-indigo-700 dark:text-indigo-300",
              image: e.icon,
              src: e.iconSrc,
              svg: e.iconSvg,
              alt: e.iconAlt
            }, null, 8, ["image", "src", "svg", "alt"])
          ])
        ]),
        s("div", Si, [
          s("h3", Ti, [
            s("span", Fi, [
              s("a", {
                href: e.href,
                class: "focus:outline-none"
              }, [
                l[0] || (l[0] = s("span", {
                  class: "absolute inset-0",
                  "aria-hidden": "true"
                }, null, -1)),
                he(" " + T(e.title), 1)
              ], 8, Ai)
            ])
          ]),
          s("p", Ii, [
            O(t.$slots, "default")
          ])
        ]),
        l[1] || (l[1] = s("div", { class: "flex-shrink-0 self-center" }, [
          s("svg", {
            class: "h-5 w-5 text-gray-400",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
              "fill-rule": "evenodd",
              d: "M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z",
              "clip-rule": "evenodd"
            })
          ])
        ], -1))
      ]);
    };
  }
}), Di = { key: 0 }, Bi = { class: "md:p-4" }, ea = /* @__PURE__ */ ue({
  __name: "EnsureAccess",
  props: {
    invalidAccess: {},
    alertClass: {}
  },
  emits: ["done"],
  setup(e) {
    const { isAuthenticated: t } = Is(), { config: l } = Ot(), n = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const u = Zt(l.value.redirectSignIn, { redirect: r });
      l.value.navigate(u);
    }, o = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const u = Zt(l.value.redirectSignOut, { ReturnUrl: r });
      l.value.navigate(u);
    };
    return (r, u) => {
      const d = U("Alert"), f = U("SecondaryButton");
      return e.invalidAccess ? (a(), i("div", Di, [
        pe(d, {
          class: g(e.alertClass),
          innerHTML: e.invalidAccess
        }, null, 8, ["class", "innerHTML"]),
        s("div", Bi, [
          G(t) ? (a(), X(f, {
            key: 1,
            onClick: o
          }, {
            default: we(() => [...u[1] || (u[1] = [
              he("Sign Out", -1)
            ])]),
            _: 1
          })) : (a(), X(f, {
            key: 0,
            onClick: n
          }, {
            default: we(() => [...u[0] || (u[0] = [
              he("Sign In", -1)
            ])]),
            _: 1
          }))
        ])
      ])) : x("", !0);
    };
  }
}), ji = { class: "absolute top-0 right-0 bg-white dark:bg-black border dark:border-gray-800 rounded normal-case text-sm shadow w-80" }, Oi = { class: "p-4" }, Hi = { class: "flex w-full justify-center" }, Ri = { key: 0 }, Ei = ["id", "value"], Ni = ["for"], zi = { key: 1 }, Ui = { class: "mb-2" }, qi = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, Qi = ["onClick"], Ki = { class: "flex" }, _i = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, Ps = /* @__PURE__ */ ue({
  __name: "FilterColumn",
  props: {
    definitions: {},
    column: {},
    topLeft: {}
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const l = e, n = t, o = A(), r = A(""), u = A(""), d = A([]), f = c(() => l.column.meta.isEnum == !0), v = c(() => Je(l.column.meta.type === "Nullable`1" ? l.column.meta.genericArgs[0] : l.column.meta.type)), p = c(() => l.column.meta.isEnum == !0 ? xs(xn(v.value.name)) : []), $ = c(() => h(l.column.type)?.map((S) => ({ key: S.value, value: S.name })) || []), b = A({ filters: [] }), y = c(() => b.value.filters);
    Cl(() => b.value = Object.assign({}, l.column.settings, {
      filters: Array.from(l.column.settings.filters)
    })), Cl(() => {
      let S = l.column.settings.filters?.[0]?.value?.split(",") || [];
      if (S.length > 0 && v.value?.isEnumInt) {
        const L = S[0] && parseInt(S[0]) || 0;
        S = v.value.enumValues?.filter((le) => (L & parseInt(le)) > 0) || [];
      }
      d.value = S;
    });
    function h(S) {
      let L = l.definitions;
      return yn(S) || (L = L.filter((le) => le.types !== "string")), L;
    }
    function k(S, L) {
      return h(S).find((le) => le.value === L);
    }
    function V() {
      if (!r.value) return;
      let S = k(l.column.type, r.value)?.name;
      S && (b.value.filters.push({ key: r.value, name: S, value: u.value }), r.value = u.value = "");
    }
    function M(S) {
      b.value.filters.splice(S, 1);
    }
    function Y(S) {
      return Vn(k(l.column.type, S.key), l.column.type, S);
    }
    function E() {
      n("done");
    }
    function D() {
      r.value = "%", o.value?.focus();
    }
    function Q() {
      if (u.value && V(), f.value) {
        let S = Object.values(d.value).filter((L) => L);
        b.value.filters = S.length > 0 ? v.value?.isEnumInt ? [{ key: "%HasAny", name: "HasAny", value: S.map((L) => parseInt(L)).reduce((L, le) => L + le, 0).toString() }] : [{ key: "%In", name: "In", value: S.join(",") }] : [];
      }
      n("save", b.value), n("done");
    }
    function q(S) {
      b.value.sort = S === b.value.sort ? void 0 : S, wt(Q);
    }
    return (S, L) => {
      const le = U("SelectInput"), ee = U("TextInput"), j = U("PrimaryButton"), K = U("SecondaryButton");
      return a(), i("div", {
        class: "fixed z-20 inset-0 overflow-y-auto",
        onClick: E,
        onVnodeMounted: D
      }, [
        s("div", {
          class: "absolute",
          style: is(`top:${e.topLeft.y}px;left:${e.topLeft.x}px`),
          onClick: L[5] || (L[5] = He(() => {
          }, ["stop"]))
        }, [
          s("div", ji, [
            s("div", Oi, [
              L[10] || (L[10] = s("h3", { class: "text-base font-medium mb-3 dark:text-gray-100" }, "Sort", -1)),
              s("div", Hi, [
                s("button", {
                  type: "button",
                  title: "Sort Ascending",
                  onClick: L[0] || (L[0] = (_) => q("ASC")),
                  class: g(`${b.value.sort === "ASC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} mr-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, [...L[6] || (L[6] = [
                  s("svg", {
                    class: "w-6 h-6",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 16 16"
                  }, [
                    s("g", { fill: "currentColor" }, [
                      s("path", {
                        "fill-rule": "evenodd",
                        d: "M10.082 5.629L9.664 7H8.598l1.789-5.332h1.234L13.402 7h-1.12l-.419-1.371h-1.781zm1.57-.785L11 2.687h-.047l-.652 2.157h1.351z"
                      }),
                      s("path", { d: "M12.96 14H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V14zm-8.46-.5a.5.5 0 0 1-1 0V3.707L2.354 4.854a.5.5 0 1 1-.708-.708l2-1.999l.007-.007a.498.498 0 0 1 .7.006l2 2a.5.5 0 1 1-.707.708L4.5 3.707V13.5z" })
                    ])
                  ], -1),
                  s("span", null, "ASC", -1)
                ])], 2),
                s("button", {
                  type: "button",
                  title: "Sort Descending",
                  onClick: L[1] || (L[1] = (_) => q("DESC")),
                  class: g(`${b.value.sort === "DESC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} ml-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, [...L[7] || (L[7] = [
                  Sl('<svg class="w-6 h-6" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16"><g fill="currentColor"><path d="M12.96 7H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V7z"></path><path fill-rule="evenodd" d="M10.082 12.629L9.664 14H8.598l1.789-5.332h1.234L13.402 14h-1.12l-.419-1.371h-1.781zm1.57-.785L11 9.688h-.047l-.652 2.156h1.351z"></path><path d="M4.5 2.5a.5.5 0 0 0-1 0v9.793l-1.146-1.147a.5.5 0 0 0-.708.708l2 1.999l.007.007a.497.497 0 0 0 .7-.006l2-2a.5.5 0 0 0-.707-.708L4.5 12.293V2.5z"></path></g></svg><span>DESC</span>', 2)
                ])], 2)
              ]),
              L[11] || (L[11] = s("h3", { class: "text-base font-medium mt-4 mb-2" }, " Filter ", -1)),
              f.value ? (a(), i("div", Ri, [
                (a(!0), i(Ve, null, Se(p.value, (_) => (a(), i("div", {
                  key: _.key,
                  class: "flex items-center"
                }, [
                  Pt(s("input", {
                    type: "checkbox",
                    id: _.key,
                    value: _.key,
                    "onUpdate:modelValue": L[2] || (L[2] = (te) => d.value = te),
                    class: "h-4 w-4 border-gray-300 rounded text-indigo-600 focus:ring-indigo-500"
                  }, null, 8, Ei), [
                    [rs, d.value]
                  ]),
                  s("label", {
                    for: _.key,
                    class: "ml-3"
                  }, T(_.value), 9, Ni)
                ]))), 128))
              ])) : (a(), i("div", zi, [
                (a(!0), i(Ve, null, Se(y.value, (_, te) => (a(), i("div", Ui, [
                  s("span", qi, [
                    he(T(e.column.name) + " " + T(_.name) + " " + T(Y(_)) + " ", 1),
                    s("button", {
                      type: "button",
                      onClick: (se) => M(te),
                      class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                    }, [...L[8] || (L[8] = [
                      s("svg", {
                        class: "h-2 w-2",
                        stroke: "currentColor",
                        fill: "none",
                        viewBox: "0 0 8 8"
                      }, [
                        s("path", {
                          "stroke-linecap": "round",
                          "stroke-width": "1.5",
                          d: "M1 1l6 6m0-6L1 7"
                        })
                      ], -1)
                    ])], 8, Qi)
                  ])
                ]))), 256)),
                s("div", Ki, [
                  pe(le, {
                    id: "filterRule",
                    class: "w-32 mr-1",
                    modelValue: r.value,
                    "onUpdate:modelValue": L[3] || (L[3] = (_) => r.value = _),
                    entries: $.value,
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue", "entries"]),
                  k(e.column.type, r.value)?.valueType !== "none" ? (a(), X(ee, {
                    key: 0,
                    ref_key: "txtFilter",
                    ref: o,
                    id: "filterValue",
                    class: "w-32 mr-1",
                    type: "text",
                    modelValue: u.value,
                    "onUpdate:modelValue": L[4] || (L[4] = (_) => u.value = _),
                    onKeyup: tn(V, ["enter"]),
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue"])) : x("", !0),
                  s("div", { class: "pt-1" }, [
                    s("button", {
                      type: "button",
                      onClick: V,
                      class: "inline-flex items-center p-1 border border-transparent rounded-full shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    }, [...L[9] || (L[9] = [
                      s("svg", {
                        class: "h-6 w-6",
                        xmlns: "http://www.w3.org/2000/svg",
                        viewBox: "0 0 20 20",
                        fill: "currentColor",
                        "aria-hidden": "true"
                      }, [
                        s("path", {
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
            s("div", _i, [
              pe(j, {
                onClick: Q,
                color: "red",
                class: "ml-2"
              }, {
                default: we(() => [...L[12] || (L[12] = [
                  he(" Save ", -1)
                ])]),
                _: 1
              }),
              pe(K, { onClick: E }, {
                default: we(() => [...L[13] || (L[13] = [
                  he(" Cancel ", -1)
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
}, lr = { class: "ml-2" }, sr = { key: 1 }, nr = { class: "pt-2" }, ar = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, or = ["onClick"], Ds = /* @__PURE__ */ ue({
  __name: "FilterViews",
  props: {
    definitions: {},
    columns: {}
  },
  emits: ["done", "change"],
  setup(e, { emit: t }) {
    const l = e, n = t, o = c(() => l.columns.filter((b) => b.settings.filters.length > 0));
    function r(b) {
      return b?.[0]?.value?.split(",");
    }
    function u(b) {
      let y = l.definitions;
      return yn(b) || (y = y.filter((h) => h.types !== "string")), y;
    }
    function d(b, y) {
      return u(b).find((h) => h.value === y);
    }
    function f(b, y) {
      return Vn(d(b.type, y.value), b.type, y);
    }
    function v(b) {
      b.settings.filters = [], n("change", b);
    }
    function p(b, y) {
      b.settings.filters.splice(y, 1), n("change", b);
    }
    function $() {
      l.columns.forEach((b) => {
        b.settings.filters = [], n("change", b);
      }), n("done");
    }
    return (b, y) => (a(), i("div", Zi, [
      s("div", Wi, [
        (a(!0), i(Ve, null, Se(o.value, (h) => (a(), i("fieldset", Gi, [
          s("legend", Ji, [
            s("span", null, T(G(De)(h.name)), 1),
            s("span", Xi, [
              s("span", Yi, [
                s("button", {
                  onClick: (k) => v(h),
                  title: `Clear all ${G(De)(h.name)} filters`,
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-red-600 hover:bg-red-200 hover:text-red-500 focus:outline-none focus:bg-red-500 focus:text-white"
                }, [...y[0] || (y[0] = [
                  s("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    s("path", {
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
            (a(!0), i(Ve, null, Se(r(h.settings.filters), (k) => (a(), i("div", {
              key: k,
              class: "flex items-center"
            }, [
              s("label", lr, T(k), 1)
            ]))), 128))
          ])) : (a(), i("div", sr, [
            (a(!0), i(Ve, null, Se(h.settings.filters, (k, V) => (a(), i("div", nr, [
              s("span", ar, [
                he(T(h.name) + " " + T(k.name) + " " + T(f(h, k)) + " ", 1),
                s("button", {
                  type: "button",
                  onClick: (M) => p(h, V),
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                }, [...y[1] || (y[1] = [
                  s("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    s("path", {
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
      s("div", { class: "flex justify-center pt-4" }, [
        s("button", {
          type: "button",
          onClick: $,
          class: "inline-flex items-center px-2.5 py-1.5 border border-gray-300 shadow-sm text-sm font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        }, [...y[2] || (y[2] = [
          s("span", null, "Clear All", -1)
        ])])
      ])
    ]));
  }
}), ir = { class: "bg-white dark:bg-black px-4 pt-5 pb-4 sm:p-6 sm:pb-4" }, rr = { class: "" }, ur = { class: "mt-3 text-center sm:mt-0 sm:mx-4 sm:text-left" }, dr = { class: "mt-4" }, cr = ["for"], fr = ["id"], vr = ["value", "selected"], mr = { class: "mt-4 flex items-center py-4 border-b border-gray-200 dark:border-gray-800" }, gr = ["id", "checked"], pr = ["for"], yr = { class: "mt-4" }, hr = { class: "pb-2 px-4" }, br = { class: "" }, wr = ["id", "value"], xr = ["for"], kr = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, Bs = /* @__PURE__ */ ue({
  __name: "QueryPrefs",
  props: {
    id: { default: "QueryPrefs" },
    columns: {},
    prefs: {},
    maxLimit: {}
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const { autoQueryGridDefaults: l } = Ot(), n = e, o = t, r = A({});
    Cl(() => r.value = Object.assign({
      take: l.value.take,
      selectedColumns: []
    }, n.prefs));
    const u = [10, 25, 50, 100, 250, 500, 1e3];
    function d() {
      o("done");
    }
    function f() {
      o("save", r.value);
    }
    return (v, p) => {
      const $ = U("PrimaryButton"), b = U("SecondaryButton"), y = U("ModalDialog");
      return a(), X(y, {
        id: e.id,
        onDone: d,
        "size-class": "w-full sm:max-w-prose"
      }, {
        default: we(() => [
          s("div", ir, [
            s("div", rr, [
              s("div", ur, [
                p[3] || (p[3] = s("h3", { class: "text-lg leading-6 font-medium text-gray-900 dark:text-gray-100" }, "Query Preferences", -1)),
                s("div", dr, [
                  s("label", {
                    for: `${e.id}-take`,
                    class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
                  }, "Results per page", 8, cr),
                  Pt(s("select", {
                    id: `${e.id}-take`,
                    "onUpdate:modelValue": p[0] || (p[0] = (h) => r.value.take = h),
                    class: "mt-1 block w-full pl-3 pr-10 py-2 text-base bg-white dark:bg-black border-gray-300 dark:border-gray-700 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                  }, [
                    (a(!0), i(Ve, null, Se(u.filter((h) => n.maxLimit == null || h <= n.maxLimit), (h) => (a(), i("option", {
                      value: h,
                      selected: h === r.value.take
                    }, T(h), 9, vr))), 256))
                  ], 8, fr), [
                    [fa, r.value.take]
                  ])
                ]),
                s("div", mr, [
                  s("input", {
                    type: "radio",
                    id: `${e.id}-allColumns`,
                    onClick: p[1] || (p[1] = (h) => r.value.selectedColumns = []),
                    checked: r.value.selectedColumns.length === 0,
                    class: "focus:ring-indigo-500 h-4 w-4 bg-white dark:bg-black text-indigo-600 dark:text-indigo-400 border-gray-300 dark:border-gray-700"
                  }, null, 8, gr),
                  s("label", {
                    class: "ml-3 block text-gray-700 dark:text-gray-300",
                    for: `${e.id}-allColumns`
                  }, "View all columns", 8, pr)
                ]),
                s("div", yr, [
                  s("div", hr, [
                    s("div", br, [
                      (a(!0), i(Ve, null, Se(e.columns, (h) => (a(), i("div", {
                        key: h.name,
                        class: "flex items-center"
                      }, [
                        Pt(s("input", {
                          type: "checkbox",
                          id: h.name,
                          value: h.name,
                          "onUpdate:modelValue": p[2] || (p[2] = (k) => r.value.selectedColumns = k),
                          class: "h-4 w-4 bg-white dark:bg-black border-gray-300 dark:border-gray-700 rounded text-indigo-600 dark:text-indigo-400 focus:ring-indigo-500"
                        }, null, 8, wr), [
                          [rs, r.value.selectedColumns]
                        ]),
                        s("label", {
                          for: h.name,
                          class: "ml-3"
                        }, T(h.name), 9, xr)
                      ]))), 128))
                    ])
                  ])
                ])
              ])
            ])
          ]),
          s("div", kr, [
            pe($, {
              onClick: f,
              color: "red",
              class: "ml-2"
            }, {
              default: we(() => [...p[4] || (p[4] = [
                he(" Save ", -1)
              ])]),
              _: 1
            }),
            pe(b, { onClick: d }, {
              default: we(() => [...p[5] || (p[5] = [
                he(" Cancel ", -1)
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
}, Vr = { key: 0 }, Mr = { key: 1 }, Sr = { key: 2 }, Tr = { key: 4 }, Fr = { class: "pl-1 pt-1 flex flex-wrap" }, Ar = { class: "flex mt-1" }, Ir = ["title"], Pr = ["disabled"], Dr = ["disabled"], Br = ["disabled"], jr = ["disabled"], Or = {
  key: 0,
  class: "flex mt-1"
}, Hr = { class: "px-4 text-lg text-black dark:text-white" }, Rr = { key: 0 }, Er = { key: 1 }, Nr = { key: 2 }, zr = { class: "flex flex-wrap" }, Ur = {
  key: 0,
  class: "pl-2 mt-1"
}, qr = {
  key: 1,
  class: "pl-2 mt-1"
}, Qr = {
  key: 2,
  class: "pl-2 mt-1"
}, Kr = {
  key: 0,
  class: "w-5 h-5 mr-1 text-green-600 dark:text-green-400",
  fill: "none",
  stroke: "currentColor",
  viewBox: "0 0 24 24",
  xmlns: "http://www.w3.org/2000/svg"
}, _r = {
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
}, eu = ["title"], tu = { class: "whitespace-nowrap" }, lu = { key: 8 }, su = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, nu = { class: "mr-1 select-none" }, au = {
  key: 1,
  class: "flex justify-between items-center"
}, ou = { class: "mr-1 select-none" }, hl = 25, iu = /* @__PURE__ */ ue({
  __name: "AutoQueryGrid",
  props: {
    filterDefinitions: {},
    id: { default: "AutoQueryGrid" },
    ctx: {},
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
    const { config: n, autoQueryGridDefaults: o } = Ot(), r = o, u = n.value.storage, d = e, f = l, v = Ne("client"), p = c(() => va(d.ctx ?? bt.createContext({
      id: d.id,
      type: d.type,
      apis: d.apis
    }))), $ = "filtering,queryString,queryFilters".split(","), b = "copyApiUrl,downloadCsv,filtersView,newItem,pagingInfo,pagingNav,preferences,refresh,resetPreferences,toolbar,forms".split(","), y = c(() => d.deny ? At($, d.deny) : At($, r.value.deny)), h = c(() => d.hide ? At(b, d.hide) : At(b, r.value.hide));
    function k(F) {
      return y.value[F];
    }
    function V(F) {
      return h.value[F];
    }
    const M = c(() => d.tableStyle ?? r.value.tableStyle), Y = c(() => d.gridClass ?? ge.getGridClass(M.value)), E = c(() => d.grid2Class ?? ge.getGrid2Class(M.value)), D = c(() => d.grid3Class ?? ge.getGrid3Class(M.value)), Q = c(() => d.grid4Class ?? ge.getGrid4Class(M.value)), q = c(() => d.tableClass ?? ge.getTableClass(M.value)), S = c(() => d.theadClass ?? ge.getTheadClass(M.value)), L = c(() => d.theadRowClass ?? ge.getTheadRowClass(M.value)), le = c(() => d.theadCellClass ?? ge.getTheadCellClass(M.value)), ee = c(() => d.toolbarButtonClass ?? ge.toolbarButtonClass);
    function j(F, I) {
      if (d.rowClass) return d.rowClass(F, I);
      const be = !!Le.value.AnyUpdate, Te = (je.value?.name ? ye(F, je.value.name) : null) == J.value;
      return ge.getTableRowClass(d.tableStyle, I, Te, be);
    }
    const K = Tl(), te = Object.keys(K).map((F) => F.toLowerCase()), se = c(() => p.value.viewModelProps.filter((F) => te.includes(F.name.toLowerCase()) || te.includes(F.name.toLowerCase() + "-header")).map((F) => F.name));
    function R() {
      let F = _t(d.selectedColumns);
      return F.length > 0 ? F : se.value.length > 0 ? se.value : [];
    }
    const C = c(() => {
      let I = R().map((re) => re.toLowerCase());
      const be = p.value.viewModelProps;
      return I.length > 0 ? I.map((re) => be.find((Te) => Te.name.toLowerCase() === re)).filter((re) => re != null) : be;
    }), P = c(() => {
      const F = p.value.viewModelProps;
      let be = R().map((ke) => ke.toLowerCase()), re = be.length > 0 ? be.map((ke) => F.find((_e) => _e.name.toLowerCase() === ke)).filter((ke) => ke != null).map((ke) => ke.name) : F.map((ke) => ke.name), Te = _t(oe.value.selectedColumns).map((ke) => ke.toLowerCase());
      return Te.length > 0 ? re.filter((ke) => Te.includes(ke.toLowerCase())) : re;
    }), de = A([]), w = Zl(new Ge()), m = Zl(new Ge()), W = A(), ve = A(!1), J = A(), z = A(), B = A(!1), ne = A(), ie = A(d.skip), ae = A(!1), oe = A({ take: hl }), me = A(!1), ce = c(() => de.value.some((F) => F.settings.filters.length > 0 || !!F.settings.sort) || oe.value.selectedColumns), fe = c(() => de.value.map((F) => F.settings.filters.length).reduce((F, I) => F + I, 0)), Fe = c(() => p.value.dataModelProps), je = c(() => p.value.dataModelPrimaryKey), xe = c(() => oe.value.take ?? hl), $e = c(() => (w.value.response ? ye(w.value.response, "results") : null) ?? []), Me = c(() => (w.value.response?.total || $e.value.length) ?? 0), qe = c(() => ie.value > 0), nt = c(() => ie.value > 0), $t = c(() => $e.value.length >= xe.value), Lt = c(() => $e.value.length >= xe.value), Qe = A(), lt = A(), at = {
      NoQuery: "No Query API was found"
    };
    t({
      update: st,
      search: js,
      createRequestArgs: El,
      reset: Us,
      createDone: el,
      createSave: ql,
      editDone: Vt,
      editSave: zt,
      forceUpdate: yt,
      setEdit: Be,
      edit: z,
      createForm: Qe,
      editForm: lt,
      apiPrefs: oe,
      results: $e,
      skip: ie,
      take: xe,
      total: Me
    }), N.interceptors.has("AutoQueryGrid.new") && N.interceptors.invoke("AutoQueryGrid.new", { props: d });
    function ze(F) {
      if (F) {
        if (d.canFilter)
          return d.canFilter(F);
        const I = Fe.value.find((be) => be.name.toLowerCase() == F.toLowerCase());
        if (I)
          return !hn(I);
      }
      return !1;
    }
    function Xe(F) {
      f("nav", F), k("queryString") && Ms(F);
    }
    async function ot(F) {
      ie.value += F, ie.value < 0 && (ie.value = 0);
      const I = Math.floor(Me.value / xe.value) * xe.value;
      ie.value > I && (ie.value = I), Xe({ skip: ie.value || void 0 }), await st();
    }
    async function Z(F, I) {
      if (z.value = null, J.value = I, !F || !I) return;
      let be = al(Le.value.AnyQuery, { [F]: I });
      const re = await v.api(be);
      if (re.succeeded) {
        let Te = ye(re.response, "results")?.[0];
        Te || console.warn(`API ${Le.value.AnyQuery?.request.name}(${F}:${I}) returned no results`), z.value = Te;
      }
    }
    async function H(F, I) {
      f("rowSelected", F, I);
      const be = je.value?.name, re = be ? ye(F, be) : null;
      !be || !re || (Xe({ edit: re }), Z(be, re));
    }
    function Ae(F, I) {
      if (!k("filtering")) return;
      let be = I.target;
      if (ze(F) && be?.tagName !== "TD") {
        let re = be?.closest("TABLE")?.getBoundingClientRect(), Te = de.value.find((ke) => ke.name.toLowerCase() == F.toLowerCase());
        if (Te && re) {
          let ke = 318, _e = re.x + ke + 10;
          ne.value = {
            column: Te,
            topLeft: {
              x: Math.max(Math.floor(I.clientX + ke / 2), _e),
              y: re.y + 45
            }
          };
        }
      }
      f("headerSelected", F, I);
    }
    function Ie() {
      ne.value = null;
    }
    async function Ke(F) {
      let I = ne.value?.column;
      I && (I.settings = F, u.setItem(vl(I.name), JSON.stringify(I.settings)), await st()), ne.value = null;
    }
    async function it(F) {
      u.setItem(vl(F.name), JSON.stringify(F.settings)), await st();
    }
    async function Rt(F) {
      B.value = !1, oe.value = F, u.setItem(Nl(), JSON.stringify(F)), await st();
    }
    function Et(F) {
      Qe.value && (Object.assign(Qe.value?.model, F), yt());
    }
    function Be(F) {
      Object.assign(z.value, F), yt();
    }
    function yt() {
      Qe.value?.forceUpdate(), lt.value?.forceUpdate(), Pe()?.proxy?.$forceUpdate();
    }
    async function st() {
      await js(El());
    }
    async function ta() {
      await st();
    }
    const la = /iPad|iPhone|iPod/.test(navigator.userAgent);
    async function js(F) {
      const I = Le.value.AnyQuery;
      if (!I) {
        console.error(at.NoQuery);
        return;
      }
      let be = al(I, F), re = await v.api(be);
      on((_e) => {
        me.value = _e, la ? wt(() => w.value = re) : w.value = re;
      })();
      let ke = ye(re.response, "results") || [];
      !re.succeeded || ke.label == 0;
    }
    function El() {
      let F = {
        include: "total",
        take: xe.value
      }, I = _t(oe.value.selectedColumns || d.selectedColumns);
      if (I.length > 0) {
        let re = je.value;
        re && !I.includes(re.name) && (I = [re.name, ...I]);
        const Te = Fe.value, ke = [];
        I.forEach((_e) => {
          const Ut = Te.find((Mt) => Mt.name.toLowerCase() == _e.toLowerCase());
          Ut?.ref?.selfId && ke.push(Ut.ref.selfId), ye(K, _e) && ke.push(...Te.filter((Mt) => Mt.ref?.selfId?.toLowerCase() == _e.toLowerCase()).map((Mt) => Mt.name));
        }), ke.forEach((_e) => {
          I.includes(_e) || I.push(_e);
        }), F.fields = Wn(I).join(",");
      }
      let be = [];
      if (de.value.forEach((re) => {
        re.settings.sort && be.push((re.settings.sort === "DESC" ? "-" : "") + re.name), re.settings.filters.forEach((Te) => {
          let ke = Te.key.replace("%", re.name);
          F[ke] = Te.value;
        });
      }), d.filters && Object.keys(d.filters).forEach((re) => {
        F[re] = d.filters[re];
      }), k("queryString") && k("queryFilters")) {
        const re = location.search ? location.search : location.hash.includes("?") ? "?" + bl(location.hash, "?") : "";
        let Te = Gl(re);
        if (Object.keys(Te).forEach((ke) => {
          C.value.find((Ut) => Ut.name.toLowerCase() === ke.toLowerCase()) && (F[ke] = Te[ke]);
        }), typeof Te.skip < "u") {
          const ke = parseInt(Te.skip);
          isNaN(ke) || (F.skip = ke);
        }
      }
      return typeof F.skip > "u" && ie.value > 0 && (F.skip = ie.value), be.length > 0 && (F.orderBy = be.join(",")), F;
    }
    function sa() {
      const F = Os("csv");
      as(F), typeof window < "u" && window.open(F);
    }
    function na() {
      const F = Os("json");
      as(F), ae.value = !0, setTimeout(() => ae.value = !1, 3e3);
    }
    function Os(F = "json") {
      const I = El(), be = `/api/${Le.value.AnyQuery?.request.name}`, re = Fa(v.baseUrl, Zt(be, { ...I, jsconfig: "edv" }));
      return re.indexOf("?") >= 0 ? Fl(re, "?") + "." + F + "?" + bl(re, "?") : re + ".json";
    }
    async function aa() {
      de.value.forEach((F) => {
        F.settings = { filters: [] }, u.removeItem(vl(F.name));
      }), oe.value = { take: hl }, u.removeItem(Nl()), await st();
    }
    function oa() {
      ve.value = !0, Xe({ create: null });
    }
    const Nt = c(() => p.value.dataModelName), Yt = c(() => d.modelTitle || Nt.value), ia = c(() => d.newButtonLabel || `New ${Yt.value}`), Nl = () => p.value.prefsCacheKey(), vl = (F) => p.value.columnCacheKey(F), { invalidAccessMessage: zl } = Is(), Hs = c(() => d.filterDefinitions || p.value.filterDefinitions), Le = c(() => p.value.apis), ml = (F) => `<span class="text-yellow-700">${F}</span>`, Rs = c(() => {
      if (!p.value.metadataApi)
        return ml(`AppMetadata not loaded, see <a class="${Ml.blue}" href="https://docs.servicestack.net/vue/use-metadata" target="_blank">useMetadata()</a>`);
      let F = p.value.invalidApis;
      if (F.length > 0)
        return ml(`Unknown API${F.length > 1 ? "s" : ""}: ${F.join(", ")}`);
      let I = Le.value;
      return I.empty ? ml("Mising DataModel in property 'type' or AutoQuery APIs to use in property 'apis'") : I.AnyQuery ? null : ml(at.NoQuery);
    }), Es = c(() => Le.value.AnyQuery && zl(Le.value.AnyQuery)), Ns = c(() => Le.value.Create && zl(Le.value.Create)), zs = c(() => Le.value.AnyUpdate && zl(Le.value.AnyUpdate)), ra = c(() => nl(Le.value.Create));
    c(() => nl(Le.value.AnyUpdate));
    const Ul = c(() => nl(Le.value.Delete));
    function Vt() {
      z.value = null, J.value = null, Xe({ edit: void 0 });
    }
    function el() {
      ve.value = !1, Xe({ create: void 0 });
    }
    async function zt() {
      await st(), Vt();
    }
    async function ql() {
      await st(), el();
    }
    function Us() {
      w.value = new Ge(), m.value = new Ge(), ve.value = !1, J.value = null, z.value = null, B.value = !1, ne.value = null, ie.value = d.skip, ae.value = !1, oe.value = { take: hl }, me.value = !1;
      const F = d.prefs || Vl(u.getItem(Nl()));
      F && (oe.value = F), de.value = C.value.map((be) => ({
        name: be.name,
        type: be.type,
        meta: be,
        settings: Object.assign(
          {
            filters: []
          },
          Vl(u.getItem(vl(be.name)))
        )
      })), isNaN(d.skip) || (ie.value = d.skip);
      let I = je.value?.name;
      if (k("queryString")) {
        const be = location.search ? location.search : location.hash.includes("?") ? "?" + bl(location.hash, "?") : "";
        let re = Gl(be);
        typeof re.create < "u" ? ve.value = typeof re.create < "u" : I && (typeof re.edit == "string" || typeof re.edit == "number") && Z(I, re.edit);
      }
      d.create === !0 && (ve.value = !0), I && d.edit != null && Z(I, d.edit);
    }
    return tt(async () => {
      Us(), await wt(), await st();
    }), (F, I) => {
      const be = U("Alert"), re = U("EnsureAccessDialog"), Te = U("AutoCreateForm"), ke = U("AutoEditForm"), _e = U("AutoViewForm"), Ut = U("ErrorSummary"), qs = U("Loading"), Mt = U("SettingsIcons"), ua = U("DataGrid");
      return Rs.value ? (a(), i("div", Cr, [
        pe(be, { innerHTML: Rs.value }, null, 8, ["innerHTML"])
      ])) : Es.value ? (a(), i("div", $r, [
        pe(ea, { "invalid-access": Es.value }, null, 8, ["invalid-access"])
      ])) : (a(), i("div", Lr, [
        V("forms") && ve.value && Le.value.Create ? (a(), i("div", Vr, [
          Ns.value ? (a(), X(re, {
            key: 0,
            title: `Create ${Yt.value}`,
            "invalid-access": Ns.value,
            "alert-class": "text-yellow-700",
            onDone: el
          }, null, 8, ["title", "invalid-access"])) : G(K).createform ? O(F.$slots, "createform", {
            key: 1,
            type: Le.value.Create.request.name,
            configure: e.configureField,
            done: el,
            save: ql
          }) : (a(), X(Te, {
            key: 2,
            ref_key: "createForm",
            ref: Qe,
            type: Le.value.Create.request.name,
            configure: e.configureField,
            onDone: el,
            onSave: ql
          }, {
            header: we(() => [
              O(F.$slots, "formheader", {
                form: "create",
                formInstance: Qe.value,
                apis: Le.value,
                type: Nt.value,
                updateModel: Et
              })
            ]),
            footer: we(() => [
              O(F.$slots, "formfooter", {
                form: "create",
                formInstance: Qe.value,
                apis: Le.value,
                type: Nt.value,
                updateModel: Et
              })
            ]),
            _: 3
          }, 8, ["type", "configure"]))
        ])) : V("forms") && z.value && Le.value.AnyUpdate ? (a(), i("div", Mr, [
          zs.value ? (a(), X(re, {
            key: 0,
            title: `Update ${Yt.value}`,
            "invalid-access": zs.value,
            "alert-class": "text-yellow-700",
            onDone: Vt
          }, null, 8, ["title", "invalid-access"])) : G(K).editform ? O(F.$slots, "editform", {
            key: 1,
            model: z.value,
            type: Le.value.AnyUpdate.request.name,
            deleteType: Ul.value ? Le.value.Delete.request.name : null,
            configure: e.configureField,
            done: Vt,
            save: zt
          }) : (a(), X(ke, {
            key: 2,
            ref_key: "editForm",
            ref: lt,
            modelValue: z.value,
            "onUpdate:modelValue": I[0] || (I[0] = (Ue) => z.value = Ue),
            type: Le.value.AnyUpdate.request.name,
            deleteType: Ul.value ? Le.value.Delete.request.name : null,
            configure: e.configureField,
            onDone: Vt,
            onSave: zt,
            onDelete: zt
          }, {
            header: we(() => [
              O(F.$slots, "formheader", {
                form: "edit",
                formInstance: lt.value,
                apis: Le.value,
                type: Nt.value,
                model: z.value,
                id: J.value,
                updateModel: Be
              })
            ]),
            footer: we(() => [
              O(F.$slots, "formfooter", {
                form: "edit",
                formInstance: lt.value,
                apis: Le.value,
                type: Nt.value,
                model: z.value,
                id: J.value,
                updateModel: Be
              })
            ]),
            _: 3
          }, 8, ["modelValue", "type", "deleteType", "configure"]))
        ])) : V("forms") && z.value ? (a(), i("div", Sr, [
          G(K).viewform ? O(F.$slots, "viewform", {
            key: 0,
            model: z.value,
            apis: Le.value,
            done: Vt
          }) : (a(), X(_e, {
            key: 1,
            model: z.value,
            apis: Le.value,
            deleteType: Ul.value ? Le.value.Delete.request.name : null,
            done: Vt,
            onSave: zt,
            onDelete: zt
          }, null, 8, ["model", "apis", "deleteType"]))
        ])) : x("", !0),
        G(K).toolbar ? O(F.$slots, "toolbar", { key: 3 }) : V("toolbar") ? (a(), i("div", Tr, [
          B.value ? (a(), X(Bs, {
            key: 0,
            columns: C.value,
            prefs: oe.value,
            onDone: I[1] || (I[1] = (Ue) => B.value = !1),
            onSave: Rt
          }, null, 8, ["columns", "prefs"])) : x("", !0),
          s("div", Fr, [
            s("div", Ar, [
              V("preferences") ? (a(), i("button", {
                key: 0,
                type: "button",
                class: "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                title: `${Yt.value} Preferences`,
                onClick: I[2] || (I[2] = (Ue) => B.value = !B.value)
              }, [...I[9] || (I[9] = [
                s("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  s("g", {
                    "stroke-width": "1.5",
                    fill: "none"
                  }, [
                    s("path", {
                      d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
                      stroke: "currentColor"
                    })
                  ])
                ], -1)
              ])], 8, Ir)) : x("", !0),
              V("pagingNav") ? (a(), i("button", {
                key: 1,
                type: "button",
                class: g(["pl-2", qe.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "First page",
                disabled: !qe.value,
                onClick: I[3] || (I[3] = (Ue) => ot(-Me.value))
              }, [...I[10] || (I[10] = [
                s("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  s("path", {
                    d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Pr)) : x("", !0),
              V("pagingNav") ? (a(), i("button", {
                key: 2,
                type: "button",
                class: g(["pl-2", nt.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Previous page",
                disabled: !nt.value,
                onClick: I[4] || (I[4] = (Ue) => ot(-xe.value))
              }, [...I[11] || (I[11] = [
                s("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  s("path", {
                    d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Dr)) : x("", !0),
              V("pagingNav") ? (a(), i("button", {
                key: 3,
                type: "button",
                class: g(["pl-2", $t.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Next page",
                disabled: !$t.value,
                onClick: I[5] || (I[5] = (Ue) => ot(xe.value))
              }, [...I[12] || (I[12] = [
                s("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  s("path", {
                    d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, Br)) : x("", !0),
              V("pagingNav") ? (a(), i("button", {
                key: 4,
                type: "button",
                class: g(["pl-2", Lt.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Last page",
                disabled: !Lt.value,
                onClick: I[6] || (I[6] = (Ue) => ot(Me.value))
              }, [...I[13] || (I[13] = [
                s("svg", {
                  class: "w-8 h-8",
                  xmlns: "http://www.w3.org/2000/svg",
                  viewBox: "0 0 24 24"
                }, [
                  s("path", {
                    d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
                    fill: "currentColor"
                  })
                ], -1)
              ])], 10, jr)) : x("", !0)
            ]),
            V("pagingInfo") ? (a(), i("div", Or, [
              s("div", Hr, [
                me.value ? (a(), i("span", Rr, "Querying...")) : x("", !0),
                $e.value.length ? (a(), i("span", Er, [
                  I[14] || (I[14] = s("span", { class: "hidden xl:inline" }, " Showing Results ", -1)),
                  he(" " + T(ie.value + 1) + " - " + T(Math.min(ie.value + $e.value.length, Me.value)) + " ", 1),
                  s("span", null, " of " + T(Me.value), 1)
                ])) : w.value.completed ? (a(), i("span", Nr, "No Results")) : x("", !0)
              ])
            ])) : x("", !0),
            s("div", zr, [
              V("refresh") ? (a(), i("div", Ur, [
                s("button", {
                  type: "button",
                  onClick: ta,
                  title: "Refresh",
                  class: g(ee.value)
                }, [...I[15] || (I[15] = [
                  s("svg", {
                    class: "w-5 h-5",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 24 24"
                  }, [
                    s("path", {
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
              V("downloadCsv") ? (a(), i("div", qr, [
                s("button", {
                  type: "button",
                  onClick: sa,
                  title: "Download CSV",
                  class: g(ee.value)
                }, [...I[16] || (I[16] = [
                  Sl('<svg class="w-5 h-5 mr-1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32"><path d="M28.781 4.405h-10.13V2.018L2 4.588v22.527l16.651 2.868v-3.538h10.13A1.162 1.162 0 0 0 30 25.349V5.5a1.162 1.162 0 0 0-1.219-1.095zm.16 21.126H18.617l-.017-1.889h2.487v-2.2h-2.506l-.012-1.3h2.518v-2.2H18.55l-.012-1.3h2.549v-2.2H18.53v-1.3h2.557v-2.2H18.53v-1.3h2.557v-2.2H18.53v-2h10.411z" fill="#20744a" fill-rule="evenodd"></path><path fill="#20744a" d="M22.487 7.439h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 10.94h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 14.441h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 17.942h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 21.443h4.323v2.2h-4.323z"></path><path fill="#fff" fill-rule="evenodd" d="M6.347 10.673l2.146-.123l1.349 3.709l1.594-3.862l2.146-.123l-2.606 5.266l2.606 5.279l-2.269-.153l-1.532-4.024l-1.533 3.871l-2.085-.184l2.422-4.663l-2.238-4.993z"></path></svg><span class="text-green-900 dark:text-green-100">Excel</span>', 2)
                ])], 2)
              ])) : x("", !0),
              V("copyApiUrl") ? (a(), i("div", Qr, [
                s("button", {
                  type: "button",
                  onClick: na,
                  title: "Copy API URL",
                  class: g(ee.value)
                }, [
                  ae.value ? (a(), i("svg", Kr, [...I[17] || (I[17] = [
                    s("path", {
                      "stroke-linecap": "round",
                      "stroke-linejoin": "round",
                      "stroke-width": "2",
                      d: "M5 13l4 4L19 7"
                    }, null, -1)
                  ])])) : (a(), i("svg", _r, [...I[18] || (I[18] = [
                    s("g", { fill: "none" }, [
                      s("path", {
                        d: "M8 4v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7.242a2 2 0 0 0-.602-1.43L16.083 2.57A2 2 0 0 0 14.685 2H10a2 2 0 0 0-2 2z",
                        stroke: "currentColor",
                        "stroke-width": "2",
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round"
                      }),
                      s("path", {
                        d: "M16 18v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h2",
                        stroke: "currentColor",
                        "stroke-width": "2",
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round"
                      })
                    ], -1)
                  ])])),
                  I[19] || (I[19] = s("span", { class: "whitespace-nowrap" }, "Copy URL", -1))
                ], 2)
              ])) : x("", !0),
              ce.value && V("resetPreferences") ? (a(), i("div", Zr, [
                s("button", {
                  type: "button",
                  onClick: aa,
                  title: "Reset Preferences & Filters",
                  class: g(ee.value)
                }, [...I[20] || (I[20] = [
                  s("svg", {
                    class: "w-5 h-5",
                    xmlns: "http://www.w3.org/2000/svg",
                    "aria-hidden": "true",
                    viewBox: "0 0 24 24"
                  }, [
                    s("path", {
                      fill: "currentColor",
                      d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
                    })
                  ], -1)
                ])], 2)
              ])) : x("", !0),
              V("filtersView") && fe.value > 0 ? (a(), i("div", Wr, [
                s("button", {
                  type: "button",
                  onClick: I[7] || (I[7] = (Ue) => W.value = W.value == "filters" ? null : "filters"),
                  class: g(ee.value),
                  "aria-expanded": "false"
                }, [
                  I[23] || (I[23] = s("svg", {
                    class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
                    "aria-hidden": "true",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 20 20",
                    fill: "currentColor"
                  }, [
                    s("path", {
                      "fill-rule": "evenodd",
                      d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
                      "clip-rule": "evenodd"
                    })
                  ], -1)),
                  s("span", Gr, T(fe.value) + " " + T(fe.value == 1 ? "Filter" : "Filters"), 1),
                  W.value != "filters" ? (a(), i("svg", Jr, [...I[21] || (I[21] = [
                    s("path", {
                      "fill-rule": "evenodd",
                      d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
                      "clip-rule": "evenodd"
                    }, null, -1)
                  ])])) : (a(), i("svg", Xr, [...I[22] || (I[22] = [
                    s("path", {
                      "fill-rule": "evenodd",
                      d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
                      "clip-rule": "evenodd"
                    }, null, -1)
                  ])]))
                ], 2)
              ])) : x("", !0),
              V("newItem") && Le.value.Create && ra.value ? (a(), i("div", Yr, [
                s("button", {
                  type: "button",
                  onClick: oa,
                  title: Yt.value,
                  class: g(ee.value)
                }, [
                  I[24] || (I[24] = s("svg", {
                    class: "w-5 h-5 mr-1 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50",
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 24 24"
                  }, [
                    s("path", {
                      d: "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z",
                      fill: "currentColor"
                    })
                  ], -1)),
                  s("span", tu, T(ia.value), 1)
                ], 10, eu)
              ])) : x("", !0),
              G(K).toolbarbuttons ? O(F.$slots, "toolbarbuttons", {
                key: 6,
                toolbarButtonClass: ee.value
              }) : x("", !0)
            ])
          ])
        ])) : x("", !0),
        W.value == "filters" ? (a(), X(Ds, {
          key: 5,
          class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
          definitions: Hs.value,
          columns: de.value,
          onDone: I[8] || (I[8] = (Ue) => W.value = null),
          onChange: it
        }, null, 8, ["definitions", "columns"])) : x("", !0),
        m.value.error ?? w.value.error ? (a(), X(Ut, {
          key: 6,
          status: m.value.error ?? w.value.error
        }, null, 8, ["status"])) : me.value ? (a(), X(qs, {
          key: 7,
          class: "p-2"
        })) : x("", !0),
        ne.value ? (a(), i("div", lu, [
          pe(Ps, {
            definitions: Hs.value,
            column: ne.value.column,
            "top-left": ne.value.topLeft,
            onDone: Ie,
            onSave: Ke
          }, null, 8, ["definitions", "column", "top-left"])
        ])) : x("", !0),
        p.value ? (a(), X(ua, {
          key: 9,
          id: e.id,
          items: $e.value,
          type: Nt.value,
          ctx: p.value,
          "selected-columns": P.value,
          class: "mt-1",
          tableStyle: M.value,
          gridClass: Y.value,
          grid2Class: E.value,
          grid3Class: D.value,
          grid4Class: Q.value,
          tableClass: q.value,
          theadClass: S.value,
          theadRowClass: L.value,
          theadCellClass: le.value,
          tbodyClass: e.tbodyClass,
          rowClass: j,
          onRowSelected: H,
          rowStyle: e.rowStyle,
          headerTitle: e.headerTitle,
          headerTitles: e.headerTitles,
          visibleFrom: e.visibleFrom,
          onHeaderSelected: Ae
        }, us({
          header: we(({ column: Ue, label: gl }) => [
            k("filtering") && ze(Ue) ? (a(), i("div", su, [
              s("span", nu, T(gl), 1),
              pe(Mt, {
                column: de.value.find((da) => da.name.toLowerCase() === Ue.toLowerCase()),
                "is-open": ne.value?.column.name === Ue
              }, null, 8, ["column", "is-open"])
            ])) : (a(), i("div", au, [
              s("span", ou, T(gl), 1)
            ]))
          ]),
          _: 2
        }, [
          Se(Object.keys(G(K)), (Ue) => ({
            name: Ue,
            fn: we((gl) => [
              O(F.$slots, Ue, $l(ln(gl)))
            ])
          }))
        ]), 1032, ["id", "items", "type", "ctx", "selected-columns", "tableStyle", "gridClass", "grid2Class", "grid3Class", "grid4Class", "tableClass", "theadClass", "theadRowClass", "theadCellClass", "tbodyClass", "rowStyle", "headerTitle", "headerTitles", "visibleFrom"])) : x("", !0)
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
}, fu = /* @__PURE__ */ ue({
  __name: "SettingsIcons",
  props: {
    column: {},
    isOpen: { type: Boolean }
  },
  setup(e) {
    return (t, l) => (a(), i("div", ru, [
      e.column?.settings?.filters?.length ? (a(), i("svg", uu, [...l[0] || (l[0] = [
        s("g", { fill: "none" }, [
          s("path", {
            d: "M3 4a1 1 0 0 1 1-1h16a1 1 0 0 1 1 1v2.586a1 1 0 0 1-.293.707l-6.414 6.414a1 1 0 0 0-.293.707V17l-4 4v-6.586a1 1 0 0 0-.293-.707L3.293 7.293A1 1 0 0 1 3 6.586V4z",
            stroke: "currentColor",
            "stroke-width": "2",
            "stroke-linecap": "round",
            "stroke-linejoin": "round"
          })
        ], -1)
      ])])) : (a(), i("svg", {
        key: 1,
        class: g(["w-4 h-4 transition-transform", e.isOpen ? "rotate-180" : ""]),
        xmlns: "http://www.w3.org/2000/svg",
        viewBox: "0 0 1024 1024"
      }, [...l[1] || (l[1] = [
        s("path", {
          d: "M505.5 658.7c3.2 4.4 9.7 4.4 12.9 0l178-246c3.8-5.3 0-12.7-6.5-12.7H643c-10.2 0-19.9 4.9-25.9 13.2L512 558.6L406.8 413.2c-6-8.3-15.6-13.2-25.9-13.2H334c-6.5 0-10.3 7.4-6.5 12.7l178 246z",
          fill: "currentColor"
        }, null, -1),
        s("path", {
          d: "M880 112H144c-17.7 0-32 14.3-32 32v736c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V144c0-17.7-14.3-32-32-32zm-40 728H184V184h656v656z",
          fill: "currentColor"
        }, null, -1)
      ])], 2)),
      e.column?.settings?.sort === "ASC" ? (a(), i("svg", du, [...l[2] || (l[2] = [
        s("g", { fill: "none" }, [
          s("path", {
            d: "M8.998 4.71L6.354 7.354a.5.5 0 1 1-.708-.707L9.115 3.18A.499.499 0 0 1 9.498 3H9.5a.5.5 0 0 1 .354.147l.01.01l3.49 3.49a.5.5 0 1 1-.707.707l-2.65-2.649V16.5a.5.5 0 0 1-1 0V4.71z",
            fill: "currentColor"
          })
        ], -1)
      ])])) : e.column?.settings?.sort === "DESC" ? (a(), i("svg", cu, [...l[3] || (l[3] = [
        s("g", { fill: "none" }, [
          s("path", {
            d: "M10.002 15.29l2.645-2.644a.5.5 0 0 1 .707.707L9.886 16.82a.5.5 0 0 1-.384.179h-.001a.5.5 0 0 1-.354-.147l-.01-.01l-3.49-3.49a.5.5 0 1 1 .707-.707l2.648 2.649V3.5a.5.5 0 0 1 1 0v11.79z",
            fill: "currentColor"
          })
        ], -1)
      ])])) : x("", !0)
    ]));
  }
}), vu = /* @__PURE__ */ ue({
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
      const n = U("EnsureAccess"), o = U("SlideOver");
      return e.invalidAccess ? (a(), X(o, {
        key: 0,
        title: e.title,
        onDone: l[0] || (l[0] = (r) => t.$emit("done")),
        "content-class": "relative flex-1"
      }, us({
        default: we(() => [
          pe(n, {
            alertClass: e.alertClass,
            invalidAccess: e.invalidAccess
          }, null, 8, ["alertClass", "invalidAccess"])
        ]),
        _: 2
      }, [
        e.subtitle ? {
          name: "subtitle",
          fn: we(() => [
            he(T(e.subtitle), 1)
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
}, wu = /* @__PURE__ */ ue({
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
    const l = (y) => ss(u.value, y.value), n = e;
    t({
      focus: r
    });
    const o = A();
    function r() {
      o.value?.focus();
    }
    const u = c(() => n.type || "text"), d = c(() => n.label ?? De(ut(n.id))), f = c(() => n.placeholder ?? d.value);
    function v(y) {
      return n.type === "range" ? y.replace("shadow-sm ", "") : y;
    }
    let p = Ne("ApiState", void 0);
    const $ = c(() => pt.call({ responseStatus: n.status ?? p?.error.value }, n.id)), b = c(() => Ct([
      rt.base,
      $.value ? rt.invalid : v(rt.valid),
      n.inputClass
    ], "TextInput", n.filterClass));
    return (y, h) => (a(), i("div", {
      class: g([y.$attrs.class])
    }, [
      O(y.$slots, "header", Ce({
        inputElement: o.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, y.$attrs)),
      d.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, T(d.value), 11, mu)) : x("", !0),
      s("div", {
        class: g(v("mt-1 relative"))
      }, [
        s("input", Ce({
          ref_key: "inputElement",
          ref: o,
          type: u.value,
          name: e.id,
          id: e.id,
          class: b.value,
          placeholder: f.value,
          value: G(ss)(u.value, e.modelValue),
          onInput: h[0] || (h[0] = (k) => y.$emit("update:modelValue", l(k.target))),
          "aria-invalid": $.value != null,
          "aria-describedby": `${e.id}-error`,
          step: "any"
        }, G(ft)(y.$attrs, ["class", "value"])), null, 16, gu),
        $.value ? (a(), i("div", pu, [...h[1] || (h[1] = [
          s("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
              "fill-rule": "evenodd",
              d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])])) : x("", !0)
      ], 2),
      $.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, T($.value), 9, yu)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, T(e.help), 9, hu)) : x("", !0),
      O(y.$slots, "footer", Ce({
        inputElement: o.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, y.$attrs))
    ], 2));
  }
}), xu = ["for"], ku = { class: "mt-1 relative" }, Cu = ["name", "id", "placeholder", "aria-invalid", "aria-describedby"], $u = ["id"], Lu = ["id"], Vu = {
  inheritAttrs: !1
}, Mu = /* @__PURE__ */ ue({
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
    const t = (f) => f.value, l = e, n = c(() => l.label ?? De(ut(l.id))), o = c(() => l.placeholder ?? n.value);
    let r = Ne("ApiState", void 0);
    const u = c(() => pt.call({ responseStatus: l.status ?? r?.error.value }, l.id)), d = c(() => Ct([
      "shadow-sm " + rt.base,
      u.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + rt.valid,
      l.inputClass
    ], "TextareaInput", l.filterClass));
    return (f, v) => (a(), i("div", {
      class: g([f.$attrs.class])
    }, [
      n.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, T(n.value), 11, xu)) : x("", !0),
      s("div", ku, [
        s("textarea", Ce({
          name: e.id,
          id: e.id,
          class: d.value,
          placeholder: o.value,
          onInput: v[0] || (v[0] = (p) => f.$emit("update:modelValue", t(p.target))),
          "aria-invalid": u.value != null,
          "aria-describedby": `${e.id}-error`
        }, G(ft)(f.$attrs, ["class"])), T(e.modelValue), 17, Cu)
      ]),
      u.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, T(u.value), 9, $u)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, T(e.help), 9, Lu)) : x("", !0)
    ], 2));
  }
}), Su = ["for"], Tu = ["id", "name", "value", "aria-invalid", "aria-describedby"], Fu = ["value"], Au = ["id"], Iu = {
  inheritAttrs: !1
}, Pu = /* @__PURE__ */ ue({
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
    const t = (f) => f.value, l = e, n = c(() => l.label ?? De(ut(l.id)));
    let o = Ne("ApiState", void 0);
    const r = c(() => pt.call({ responseStatus: l.status ?? o?.error.value }, l.id)), u = c(() => l.entries || (l.values ? l.values.map((f) => ({ key: f, value: f })) : l.options ? Object.keys(l.options).map((f) => ({ key: f, value: l.options[f] })) : [])), d = c(() => Ct([
      "mt-1 block w-full pl-3 pr-10 py-2 text-base focus:outline-none sm:text-sm rounded-md dark:text-white dark:bg-gray-900 dark:border-gray-600 disabled:bg-slate-50 dark:disabled:bg-slate-900 disabled:text-slate-500 disabled:border-slate-200 dark:disabled:border-slate-700 disabled:shadow-none",
      r.value ? "border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500" : "shadow-sm border-gray-300 text-gray-900 focus:ring-indigo-500 focus:border-indigo-500",
      l.inputClass
    ], "SelectInput", l.filterClass));
    return (f, v) => (a(), i("div", {
      class: g([f.$attrs.class])
    }, [
      n.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, T(n.value), 11, Su)) : x("", !0),
      s("select", Ce({
        id: e.id,
        name: e.id,
        class: d.value,
        value: e.modelValue,
        onInput: v[0] || (v[0] = (p) => f.$emit("update:modelValue", t(p.target))),
        "aria-invalid": r.value != null,
        "aria-describedby": `${e.id}-error`
      }, G(ft)(f.$attrs, ["class"])), [
        (a(!0), i(Ve, null, Se(u.value, (p) => (a(), i("option", {
          value: p.key
        }, T(p.value), 9, Fu))), 256))
      ], 16, Tu),
      r.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, T(r.value), 9, Au)) : x("", !0)
    ], 2));
  }
}), Du = { class: "flex items-center h-5" }, Bu = ["id", "name", "checked"], ju = { class: "ml-3 text-sm" }, Ou = ["for"], Hu = {
  key: 0,
  class: "mt-2 text-sm text-red-500",
  id: "`${id}-error`"
}, Ru = {
  key: 1,
  class: "mt-2 text-sm text-gray-500",
  id: "`${id}-description`"
}, Eu = {
  inheritAttrs: !1
}, Nu = /* @__PURE__ */ ue({
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
    const l = e, n = c(() => l.label ?? De(ut(l.id)));
    let o = Ne("ApiState", void 0);
    const r = c(() => pt.call({ responseStatus: l.status ?? o?.error.value }, l.id)), u = c(() => Ct(["focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800", l.inputClass], "CheckboxInput", l.filterClass));
    return (d, f) => (a(), i("div", {
      class: g(["relative flex items-start", d.$attrs.class])
    }, [
      s("div", Du, [
        s("input", Ce({
          id: e.id,
          name: e.id,
          type: "checkbox",
          checked: e.modelValue,
          onInput: f[0] || (f[0] = (v) => d.$emit("update:modelValue", v.target.checked)),
          class: u.value
        }, G(ft)(d.$attrs, ["class"])), null, 16, Bu)
      ]),
      s("div", ju, [
        s("label", {
          for: e.id,
          class: g(`font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, T(n.value), 11, Ou),
        r.value ? (a(), i("p", Hu, T(r.value), 1)) : e.help ? (a(), i("p", Ru, T(e.help), 1)) : x("", !0)
      ])
    ], 2));
  }
}), zu = ["id"], Uu = ["for"], qu = { class: "mt-1 relative" }, Qu = ["id", "name", "value"], Ku = { class: "flex flex-wrap pb-1.5" }, _u = { class: "pt-1.5 pl-1" }, Zu = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 dark:bg-indigo-800 text-indigo-700 dark:text-indigo-300" }, Wu = ["onClick"], Gu = { class: "pt-1.5 pl-1 shrink" }, Ju = ["type", "name", "id", "aria-invalid", "aria-describedby"], Xu = ["id"], Yu = ["onMouseover", "onClick"], ed = { class: "block truncate" }, td = {
  key: 1,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, ld = ["id"], sd = ["id"], nd = {
  inheritAttrs: !1
}, ad = /* @__PURE__ */ ue({
  ...nd,
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
    const l = e, n = t;
    function o(C) {
      return l.converter ? l.converter(C) : C;
    }
    const r = c(() => Ee(o(l.modelValue), (C) => typeof C == "string" ? C.trim().length == 0 ? [] : C.split(",") : C) || []), u = A(), d = A(!1), f = c(() => {
      const C = $.value.toLowerCase();
      return !l.allowableValues || l.allowableValues.length == 0 ? [] : l.allowableValues.length < 1e3 ? l.allowableValues.filter((P) => !r.value.includes(P) && P.toLowerCase().includes(C)) : l.allowableValues.filter((P) => !r.value.includes(P) && P.startsWith(C));
    });
    function v(C) {
      u.value = C;
    }
    const p = A(null), $ = A(""), b = c(() => l.type || "text"), y = c(() => l.label ?? De(ut(l.id)));
    let h = Ne("ApiState", void 0);
    const k = c(() => pt.call({ responseStatus: l.status ?? h?.error.value }, l.id)), V = c(() => Ct([
      "w-full cursor-text flex flex-wrap sm:text-sm rounded-md dark:text-white dark:bg-gray-900 border focus-within:border-transparent focus-within:ring-1 focus-within:outline-none",
      k.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus-within:outline-none focus-within:ring-red-500 focus-within:border-red-500" : "shadow-sm border-gray-300 dark:border-gray-600 focus-within:ring-indigo-500 focus-within:border-indigo-500",
      l.inputClass
    ], "TagInput", l.filterClass)), M = (C) => S(r.value.filter((P) => P != C));
    function Y(C) {
      document.activeElement === C.target && p.value?.focus();
    }
    const E = A();
    function D() {
      d.value = !0, E.value = !0;
    }
    function Q() {
      D();
    }
    function q() {
      te(le()), E.value = !1, setTimeout(() => {
        E.value || (d.value = !1);
      }, 200);
    }
    function S(C) {
      const P = l.string ? C.join(",") : C;
      n("update:modelValue", P);
    }
    function L(C) {
      if (C.key == "Backspace" && $.value.length == 0 && r.value.length > 0 && M(r.value[r.value.length - 1]), !(!l.allowableValues || l.allowableValues.length == 0))
        if (C.code == "Escape" || C.code == "Tab")
          d.value = !1;
        else if (C.code == "Home")
          u.value = f.value[0], K();
        else if (C.code == "End")
          u.value = f.value[f.value.length - 1], K();
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
        } else C.code == "Enter" ? u.value && d.value ? (te(u.value), C.preventDefault()) : d.value = !1 : d.value = f.value.length > 0;
    }
    function le() {
      if ($.value.length == 0) return "";
      let C = Aa($.value.trim(), ",");
      return C[0] == "," && (C = C.substring(1)), C = C.trim(), C.length == 0 && d.value && f.value.length > 0 ? u.value : C;
    }
    function ee(C) {
      const P = le();
      if (P.length > 0) {
        const de = l.delimiters.some((m) => m == C.key);
        if (de && C.preventDefault(), C.key == "Enter" || C.key == "NumpadEnter" || C.key.length == 1 && de) {
          te(P);
          return;
        }
      }
    }
    const j = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function K() {
      setTimeout(() => {
        let C = Ll(`#${l.id}-tag li.active`);
        C && C.scrollIntoView(j);
      }, 0);
    }
    function _() {
      setTimeout(() => {
        let C = Ll(`#${l.id}-tag li.active`);
        C && ("scrollIntoViewIfNeeded" in C ? C.scrollIntoViewIfNeeded(j) : C.scrollIntoView(j));
      }, 0);
    }
    function te(C) {
      if (C.length === 0) return;
      const P = Array.from(r.value);
      P.indexOf(C) == -1 && P.push(C), S(P), $.value = "", d.value = !1;
    }
    function se(C) {
      const P = C.clipboardData?.getData("Text");
      R(P);
    }
    function R(C) {
      if (!C) return;
      const P = new RegExp(`\\n|\\t|${l.delimiters.join("|")}`), de = Array.from(r.value);
      C.split(P).map((m) => m.trim()).forEach((m) => {
        de.indexOf(m) == -1 && de.push(m);
      }), S(de), $.value = "";
    }
    return (C, P) => (a(), i("div", {
      class: g([C.$attrs.class]),
      id: `${e.id}-tag`,
      onmousemove: "cancelBlur=true"
    }, [
      y.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, T(y.value), 11, Uu)) : x("", !0),
      s("div", qu, [
        s("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: r.value.join(",")
        }, null, 8, Qu),
        s("button", {
          class: g(V.value),
          onClick: He(Y, ["prevent"]),
          onFocus: P[2] || (P[2] = (de) => d.value = !0),
          tabindex: "-1"
        }, [
          s("div", Ku, [
            (a(!0), i(Ve, null, Se(r.value, (de) => (a(), i("div", _u, [
              s("span", Zu, [
                he(T(de) + " ", 1),
                s("button", {
                  type: "button",
                  onClick: (w) => M(de),
                  class: "flex-shrink-0 ml-1 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 dark:text-indigo-500 hover:bg-indigo-200 dark:hover:bg-indigo-800 hover:text-indigo-500 dark:hover:text-indigo-400 focus:outline-none focus:bg-indigo-500 focus:text-white dark:focus:text-black"
                }, [...P[3] || (P[3] = [
                  s("svg", {
                    class: "h-2 w-2",
                    stroke: "currentColor",
                    fill: "none",
                    viewBox: "0 0 8 8"
                  }, [
                    s("path", {
                      "stroke-linecap": "round",
                      "stroke-width": "1.5",
                      d: "M1 1l6 6m0-6L1 7"
                    })
                  ], -1)
                ])], 8, Wu)
              ])
            ]))), 256)),
            s("div", Gu, [
              Pt(s("input", Ce({
                ref_key: "txtInput",
                ref: p,
                type: b.value,
                role: "combobox",
                "aria-controls": "options",
                "aria-expanded": "false",
                autocomplete: "off",
                spellcheck: "false",
                name: `${e.id}-txt`,
                id: `${e.id}-txt`,
                class: "p-0 dark:bg-transparent rounded-md border-none focus:!border-none focus:!outline-none",
                style: `box-shadow:none !important;width:${$.value.length + 1}ch`,
                "onUpdate:modelValue": P[0] || (P[0] = (de) => $.value = de),
                "aria-invalid": k.value != null,
                "aria-describedby": `${e.id}-error`,
                onKeydown: L,
                onKeypress: ee,
                onPaste: He(se, ["prevent", "stop"]),
                onFocus: Q,
                onBlur: q,
                onClick: P[1] || (P[1] = (de) => d.value = !0)
              }, G(ft)(C.$attrs, ["class", "required"])), null, 16, Ju), [
                [ma, $.value]
              ])
            ])
          ])
        ], 34),
        d.value && f.value.length ? (a(), i("ul", {
          key: 0,
          class: "absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm",
          onKeydown: L,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), i(Ve, null, Se(f.value.slice(0, e.maxVisibleItems), (de) => (a(), i("li", {
            class: g([de === u.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (w) => v(de),
            onClick: (w) => te(de),
            role: "option",
            tabindex: "-1"
          }, [
            s("span", ed, T(de), 1)
          ], 42, Yu))), 256))
        ], 40, Xu)) : x("", !0),
        k.value ? (a(), i("div", td, [...P[4] || (P[4] = [
          s("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
              "fill-rule": "evenodd",
              d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])])) : x("", !0)
      ]),
      k.value ? (a(), i("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, T(k.value), 9, ld)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, T(e.help), 9, sd)) : x("", !0)
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
}, Vd = /* @__PURE__ */ ue({
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
    const t = e, l = A(null), { assetsPathResolver: n, fallbackPathResolver: o } = Ot(), r = {}, u = A(), d = A(t.files?.map(f) || []);
    function f(D) {
      return D.filePath = n(D.filePath), D;
    }
    t.values && t.values.length > 0 && (d.value = t.values.map((D) => {
      let Q = D.replace(/\\/g, "/");
      return { fileName: an(Dt(Q, "/"), "."), filePath: Q, contentType: Yl(Q) };
    }).map(f));
    const v = c(() => t.label ?? De(ut(t.id))), p = c(() => t.placeholder ?? v.value);
    let $ = Ne("ApiState", void 0);
    const b = c(() => pt.call({ responseStatus: t.status ?? $?.error.value }, t.id)), y = c(() => Ct([
      "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-violet-50 dark:file:bg-violet-900 file:text-violet-700 dark:file:text-violet-200 hover:file:bg-violet-100 dark:hover:file:bg-violet-800",
      b.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500" : "text-slate-500 dark:text-slate-400",
      t.inputClass
    ], "FileInput", t.filterClass)), h = (D) => {
      let Q = D.target;
      u.value = "", d.value = Array.from(Q.files || []).map((q) => ({
        fileName: q.name,
        filePath: vs(q),
        contentLength: q.size,
        contentType: q.type || Yl(q.name)
      }));
    }, k = () => l.value?.click(), V = (D) => D == null ? !1 : D.startsWith("data:") || D.startsWith("blob:"), M = c(() => {
      if (d.value.length > 0)
        return d.value[0].filePath;
      let D = typeof t.modelValue == "string" ? t.modelValue : t.values && t.values[0];
      return D && Tt(n(D)) || null;
    }), Y = (D) => !D || D.startsWith("data:") || D.endsWith(".svg") ? "" : "rounded-full object-cover";
    function E(D) {
      u.value = o(M.value);
    }
    return jt(vn), (D, Q) => (a(), i("div", {
      class: g(["flex", e.multiple ? "flex-col" : "justify-between"])
    }, [
      s("div", od, [
        v.value ? (a(), i("label", {
          key: 0,
          for: e.id,
          class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, T(v.value), 11, id)) : x("", !0),
        s("div", rd, [
          s("span", ud, T(e.help ?? v.value), 1),
          s("input", Ce({
            ref_key: "input",
            ref: l,
            type: "file",
            multiple: e.multiple,
            name: e.id,
            id: e.id,
            class: y.value,
            placeholder: p.value,
            "aria-invalid": b.value != null,
            "aria-describedby": `${e.id}-error`
          }, D.$attrs, { onChange: h }), null, 16, dd),
          b.value ? (a(), i("div", cd, [...Q[0] || (Q[0] = [
            s("svg", {
              class: "h-5 w-5 text-red-500",
              xmlns: "http://www.w3.org/2000/svg",
              viewBox: "0 0 20 20",
              fill: "currentColor",
              "aria-hidden": "true"
            }, [
              s("path", {
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
        }, T(b.value), 9, fd)) : e.help ? (a(), i("p", {
          key: 2,
          class: "mt-2 text-sm text-gray-500",
          id: `${e.id}-description`
        }, T(e.help), 9, vd)) : x("", !0)
      ]),
      e.multiple ? (a(), i("div", yd, [
        s("table", hd, [
          (a(!0), i(Ve, null, Se(d.value, (q) => (a(), i("tr", null, [
            s("td", bd, [
              s("div", {
                class: "flex w-full",
                title: V(q.filePath) ? "" : q.filePath
              }, [
                s("img", {
                  src: r[G(Tt)(q.filePath)] || G(n)(G(Tt)(q.filePath)),
                  class: g(["mr-2 h-8 w-8", Y(q.filePath)]),
                  onError: (S) => r[G(Tt)(q.filePath)] = G(o)(G(Tt)(q.filePath))
                }, null, 42, xd),
                V(q.filePath) ? (a(), i("span", Cd, T(q.fileName), 1)) : (a(), i("a", {
                  key: 0,
                  href: G(n)(q.filePath || ""),
                  target: "_blank",
                  class: "overflow-hidden"
                }, T(q.fileName), 9, kd))
              ], 8, wd)
            ]),
            s("td", $d, [
              q.contentLength && q.contentLength > 0 ? (a(), i("span", Ld, T(G(gs)(q.contentLength)), 1)) : x("", !0)
            ])
          ]))), 256))
        ])
      ])) : (a(), i("div", md, [
        M.value ? (a(), i("div", {
          key: 0,
          class: "shrink-0 cursor-pointer",
          title: V(M.value) ? "" : M.value
        }, [
          s("img", {
            onClick: k,
            class: g(["h-16 w-16", Y(M.value)]),
            alt: `Current ${v.value ?? ""}`,
            src: u.value || G(n)(M.value),
            onError: E
          }, null, 42, pd)
        ], 8, gd)) : x("", !0)
      ]))
    ], 2));
  }
}), Md = ["id"], Sd = ["for"], Td = { class: "relative mt-1" }, Fd = ["id", "placeholder", "readonly"], Ad = ["id"], Id = ["onMouseover", "onClick"], Pd = {
  key: 2,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none",
  tabindex: "-1"
}, Dd = ["id"], Bd = ["id"], jd = /* @__PURE__ */ ue({
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
    const n = A(!1), o = e, r = l;
    t({ toggle: j });
    function u(R) {
      return Array.isArray(o.modelValue) && o.modelValue.indexOf(R) >= 0;
    }
    const d = c(() => o.label ?? De(ut(o.id)));
    let f = Ne("ApiState", void 0);
    const v = c(() => pt.call({ responseStatus: o.status ?? f?.error.value }, o.id)), p = c(() => [rt.base, v.value ? rt.invalid : rt.valid]), $ = A(null), b = A(""), y = A(null), h = A(o.viewCount), k = A([]), V = c(() => b.value ? o.options.filter((C) => o.match(C, b.value)).slice(0, h.value) : o.options), M = ["Tab", "Escape", "ArrowDown", "ArrowUp", "Enter", "PageUp", "PageDown", "Home", "End"];
    function Y(R) {
      y.value = R, k.value.indexOf(R) > Math.floor(h.value * 0.9) && (h.value += o.viewCount, se());
    }
    const E = [",", `
`, "	"];
    function D(R) {
      const C = R.clipboardData?.getData("Text");
      Q(C);
    }
    function Q(R) {
      if (!R) return;
      const C = E.some((P) => R.includes(P));
      if (!o.multiple || !C) {
        const P = o.options.filter((de) => o.match(de, R));
        P.length == 1 && (te(P[0]), n.value = !1, kl());
      } else if (C) {
        const P = new RegExp("\\r|\\n|\\t|,"), w = R.split(P).filter((m) => m.trim()).map((m) => o.options.find((W) => o.match(W, m))).filter((m) => !!m);
        if (w.length > 0) {
          b.value = "", n.value = !1, y.value = null;
          let m = Array.from(o.modelValue || []);
          w.forEach((W) => {
            u(W) ? m = m.filter((ve) => ve != W) : m.push(W);
          }), r("update:modelValue", m), kl();
        }
      }
    }
    function q(R) {
      M.indexOf(R.code) || _();
    }
    function S(R) {
      if (!(R.shiftKey || R.ctrlKey || R.altKey)) {
        if (!n.value) {
          R.code == "ArrowDown" && (n.value = !0, y.value = k.value[0]);
          return;
        }
        if (R.code == "Escape")
          n.value && (R.stopPropagation(), n.value = !1);
        else if (R.code == "Tab")
          n.value = !1;
        else if (R.code == "Home")
          y.value = k.value[0], le();
        else if (R.code == "End")
          y.value = k.value[k.value.length - 1], le();
        else if (R.code == "ArrowDown") {
          if (!y.value)
            y.value = k.value[0];
          else {
            const C = k.value.indexOf(y.value);
            y.value = C + 1 < k.value.length ? k.value[C + 1] : k.value[0];
          }
          ee();
        } else if (R.code == "ArrowUp") {
          if (!y.value)
            y.value = k.value[k.value.length - 1];
          else {
            const C = k.value.indexOf(y.value);
            y.value = C - 1 >= 0 ? k.value[C - 1] : k.value[k.value.length - 1];
          }
          ee();
        } else R.code == "Enter" && (y.value ? (te(y.value), o.multiple || (R.preventDefault(), kl())) : n.value = !1);
      }
    }
    const L = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function le() {
      setTimeout(() => {
        let R = Ll(`#${o.id}-autocomplete li.active`);
        R && R.scrollIntoView(L);
      }, 0);
    }
    function ee() {
      setTimeout(() => {
        let R = Ll(`#${o.id}-autocomplete li.active`);
        R && ("scrollIntoViewIfNeeded" in R ? R.scrollIntoViewIfNeeded(L) : R.scrollIntoView(L));
      }, 0);
    }
    function j(R) {
      n.value = R, R && (se(), $.value?.focus());
    }
    function K() {
      !o.multiple && o.modelValue ? (n.value = !n.value, n.value && se()) : _();
    }
    function _() {
      n.value = !0, se();
    }
    function te(R) {
      if (b.value = "", n.value = !1, o.multiple) {
        let C = Array.from(o.modelValue || []);
        u(R) ? C = C.filter((P) => P != R) : C.push(R), y.value = null, r("update:modelValue", C);
      } else
        r("update:modelValue", R);
    }
    function se() {
      k.value = V.value;
    }
    return kt(b, se), (R, C) => (a(), i("div", {
      id: `${e.id}-autocomplete`
    }, [
      d.value ? (a(), i("label", {
        key: 0,
        for: `${e.id}-text`,
        class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
      }, T(d.value), 9, Sd)) : x("", !0),
      s("div", Td, [
        Pt(s("input", Ce({
          ref_key: "txtInput",
          ref: $,
          id: `${e.id}-text`,
          type: "text",
          role: "combobox",
          "aria-controls": "options",
          "aria-expanded": "false",
          autocomplete: "off",
          spellcheck: "false",
          "onUpdate:modelValue": C[0] || (C[0] = (P) => b.value = P),
          class: p.value,
          placeholder: e.multiple || !e.modelValue ? e.placeholder : "",
          readonly: !e.multiple && !!e.modelValue && !n.value,
          onKeydown: S,
          onKeyup: q,
          onClick: K,
          onPaste: D,
          required: !1
        }, R.$attrs), null, 16, Fd), [
          [ga, b.value]
        ]),
        s("button", {
          type: "button",
          onClick: C[1] || (C[1] = (P) => j(!n.value)),
          class: "absolute inset-y-0 right-0 flex items-center rounded-r-md px-2 focus:outline-none",
          tabindex: "-1"
        }, [...C[2] || (C[2] = [
          s("svg", {
            class: "h-5 w-5 text-gray-400 dark:text-gray-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
              "fill-rule": "evenodd",
              d: "M10 3a.75.75 0 01.55.24l3.25 3.5a.75.75 0 11-1.1 1.02L10 4.852 7.3 7.76a.75.75 0 01-1.1-1.02l3.25-3.5A.75.75 0 0110 3zm-3.76 9.2a.75.75 0 011.06.04l2.7 2.908 2.7-2.908a.75.75 0 111.1 1.02l-3.25 3.5a.75.75 0 01-1.1 0l-3.25-3.5a.75.75 0 01.04-1.06z",
              "clip-rule": "evenodd"
            })
          ], -1)
        ])]),
        n.value ? (a(), i("ul", {
          key: 0,
          class: "absolute z-20 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm",
          onKeydown: S,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), i(Ve, null, Se(k.value, (P) => (a(), i("li", {
            class: g([P === y.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (de) => Y(P),
            onClick: (de) => te(P),
            role: "option",
            tabindex: "-1"
          }, [
            typeof P == "string" ? O(R.$slots, "item", Ce({
              key: 0,
              ref_for: !0
            }, { key: P, value: P })) : O(R.$slots, "item", Ce({
              key: 1,
              ref_for: !0
            }, P)),
            u(P) ? (a(), i("span", {
              key: 2,
              class: g(["absolute inset-y-0 right-0 flex items-center pr-4", P === y.value ? "text-white" : "text-indigo-600"])
            }, [...C[3] || (C[3] = [
              s("svg", {
                class: "h-5 w-5",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                s("path", {
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
          class: "h-8 -mt-8 ml-3 pt-0.5 pointer-events-none"
        }, [
          typeof e.modelValue == "string" ? O(R.$slots, "item", $l(Ce({ key: 0 }, { key: e.modelValue, value: e.modelValue }))) : O(R.$slots, "item", $l(Ce({ key: 1 }, e.modelValue)))
        ], 32)) : x("", !0),
        v.value ? (a(), i("div", Pd, [...C[4] || (C[4] = [
          s("svg", {
            class: "h-5 w-5 text-red-500",
            xmlns: "http://www.w3.org/2000/svg",
            viewBox: "0 0 20 20",
            fill: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
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
      }, T(v.value), 9, Dd)) : e.help ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, T(e.help), 9, Bd)) : x("", !0)
    ], 8, Md));
  }
}), Od = ["id", "name", "value"], Hd = { class: "block truncate" }, Rd = /* @__PURE__ */ ue({
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
    const n = e;
    t({
      toggle(y) {
        d.value?.toggle(y);
      }
    });
    const o = l;
    function r(y) {
      o("update:modelValue", y);
    }
    const u = c(() => n.multiple != null ? n.multiple : Array.isArray(n.modelValue)), d = A();
    function f(y, h) {
      return !h || y.value.toLowerCase().includes(h.toLowerCase());
    }
    const v = c(() => n.entries || (n.values ? n.values.map((y) => ({ key: y, value: y })) : n.options ? Object.keys(n.options).map((y) => ({ key: y, value: n.options[y] })) : [])), p = A(u.value ? [] : null);
    function $() {
      let y = n.modelValue && typeof n.modelValue == "object" && !Array.isArray(n.modelValue) ? n.modelValue.key : n.modelValue;
      y == null || y === "" ? p.value = u.value ? [] : null : typeof y == "string" ? p.value = v.value.find((h) => h.key === y) || null : Array.isArray(y) && (p.value = v.value.filter((h) => y.includes(h.key)));
    }
    tt($);
    const b = c(() => p.value == null ? "" : Array.isArray(p.value) ? p.value.map((y) => encodeURIComponent(y.key)).join(",") : p.value.key);
    return (y, h) => {
      const k = U("Autocomplete");
      return a(), i(Ve, null, [
        s("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: b.value
        }, null, 8, Od),
        pe(k, Ce({
          ref_key: "input",
          ref: d,
          id: e.id,
          options: v.value,
          match: f,
          multiple: u.value
        }, y.$attrs, {
          modelValue: p.value,
          "onUpdate:modelValue": [
            h[0] || (h[0] = (V) => p.value = V),
            r
          ]
        }), {
          item: we(({ key: V, value: M }) => [
            s("span", Hd, T(M), 1)
          ]),
          _: 1
        }, 16, ["id", "options", "multiple", "modelValue"])
      ], 64);
    };
  }
}), Ed = /* @__PURE__ */ ue({
  __name: "DynamicInput",
  props: {
    input: {},
    modelValue: {},
    api: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, n = t, o = c(() => l.input.type || "text"), r = "ignore,css,options,meta,allowableValues,allowableEntries,op,prop,type,id,name".split(","), u = c(() => ft(l.input, r)), d = A(o.value === "file" ? null : l.modelValue[l.input.id]);
    kt(d, () => {
      l.modelValue[l.input.id] = d.value, n("update:modelValue", l.modelValue);
    });
    const f = c(() => {
      const v = l.modelValue[l.input.id];
      if (l.input.type !== "file" || !v) return [];
      if (typeof v == "string") return [{ filePath: v, fileName: Dt(v, "/") }];
      if (!Array.isArray(v) && typeof v == "object") return v;
      if (Array.isArray(v)) {
        const p = [];
        return v.forEach(($) => {
          typeof $ == "string" ? p.push({ filePath: $, fileName: Dt($, "/") }) : typeof $ == "object" && p.push($);
        }), p;
      }
    });
    return (v, p) => {
      const $ = U("SelectInput"), b = U("CheckboxInput"), y = U("TagInput"), h = U("Combobox"), k = U("FileInput"), V = U("TextareaInput"), M = U("MarkdownInput"), Y = U("TextInput");
      return G(N).component(o.value) ? (a(), X(sn(G(N).component(o.value)), Ce({
        key: 0,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[0] || (p[0] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "select" ? (a(), X($, Ce({
        key: 1,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[1] || (p[1] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o.value == "checkbox" ? (a(), X(b, Ce({
        key: 2,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[2] || (p[2] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "tag" ? (a(), X(y, Ce({
        key: 3,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[3] || (p[3] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        allowableValues: e.input.allowableValues,
        string: e.input.prop?.type == "String"
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "allowableValues", "string"])) : o.value == "combobox" ? (a(), X(h, Ce({
        key: 4,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[4] || (p[4] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o.value == "file" ? (a(), X(k, Ce({
        key: 5,
        id: e.input.id,
        status: e.api?.error,
        modelValue: d.value,
        "onUpdate:modelValue": p[5] || (p[5] = (E) => d.value = E),
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label,
        files: f.value
      }, u.value), null, 16, ["id", "status", "modelValue", "input-class", "label-class", "files"])) : o.value == "textarea" ? (a(), X(V, Ce({
        key: 6,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[6] || (p[6] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o.value == "MarkdownInput" ? (a(), X(M, Ce({
        key: 7,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[7] || (p[7] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : (a(), X(Y, Ce({
        key: 8,
        type: o.value,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": p[8] || (p[8] = (E) => d.value = E),
        status: e.api?.error,
        "input-class": e.input.css?.input,
        "label-class": e.input.css?.label
      }, u.value), null, 16, ["type", "id", "modelValue", "status", "input-class", "label-class"]));
    };
  }
}), Nd = { class: "lookup-field" }, zd = ["name", "value"], Ud = {
  key: 0,
  class: "flex justify-between"
}, qd = ["for"], Qd = {
  key: 0,
  class: "flex items-center"
}, Kd = { class: "text-sm text-gray-500 dark:text-gray-400 pr-1" }, _d = {
  key: 1,
  class: "mt-1 relative"
}, Zd = { class: "w-full inline-flex truncate" }, Wd = { class: "text-blue-700 dark:text-blue-300 flex cursor-pointer" }, Gd = ["id"], Jd = ["id"], Xd = /* @__PURE__ */ ue({
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
    const { config: l } = Ot(), { metadataApi: n } = vt(), o = e, r = t, u = c(() => o.id || o.input.id), d = c(() => o.label ?? De(ut(u.value)));
    let f = Ne("ApiState", void 0);
    const v = Ne("client"), p = c(() => pt.call({ responseStatus: o.status ?? f?.error.value }, u.value)), $ = A(""), b = A(""), y = c(() => ye(o.modelValue, u.value)), h = c(() => et(o.metadataType).find((Q) => Q.name.toLowerCase() == u.value.toLowerCase())), k = c(() => Je(h.value?.ref?.model)?.icon || l.value.tableIcon);
    function V(Q) {
      return Q ? o.input.options ? Object.assign({}, Q, Hl(o.input.options, {
        input: o.input,
        $typeFields: et(o.metadataType).map((q) => q.name),
        ...N.config.scopeWhitelist
      })) : Q : null;
    }
    const M = c(() => V(h.value?.ref ?? (o.input.type == "lookup" ? {
      model: o.metadataType.name,
      refId: Jt(o.metadataType)?.name ?? "id",
      refLabel: o.metadataType.properties?.find((Q) => Q.type == "String" && !Q.isPrimaryKey)?.name
    } : null)));
    let Y;
    function E(Q) {
      if (Q) {
        if (Y == null) {
          console.warn("No ModalProvider required by LookupInput");
          return;
        }
        Y.openModal({ name: "ModalLookup", ref: Q }, (q) => {
          if (console.debug("openModal", $.value, " -> ", q, We.setRefValue(Q, q), Q), q) {
            const S = ye(q, Q.refId);
            $.value = We.setRefValue(Q, q) || S;
            const L = G(o.modelValue);
            L[u.value] = S, r("update:modelValue", L);
          }
        });
      }
    }
    function D() {
      o.modelValue[u.value] = null, $.value = "";
    }
    return tt(async () => {
      Y = Ne("ModalProvider", void 0);
      const Q = o.modelValue;
      o.modelValue[u.value] || (o.modelValue[u.value] = null);
      const q = h.value, S = M.value;
      if (!q || !S) {
        console.warn(`No RefInfo for property '${u.value}'`);
        return;
      }
      $.value = "";
      let L = S.selfId == null ? ye(Q, q.name) : ye(Q, S.selfId);
      if (rl(L) && (L = ye(Q, S.refId)), L == null)
        return;
      const ee = n.value?.operations.find((j) => j.dataModel?.name == S.model);
      if (console.debug("LookupInput queryOp", ee), ee != null) {
        const j = ye(Q, q.name);
        if (rl(j)) return;
        if ($.value = `${j}`, b.value = q.name, S.refLabel != null) {
          const K = et(o.metadataType).filter((se) => se.type == S.model);
          K.length || console.warn(`Could not find ${S.model} Property on ${o.metadataType.name}`);
          const _ = K.map((se) => ye(Q, se.name)).filter((se) => !!se), te = _.length <= 1 ? _[0] : _.find((se) => se[S.refId ?? "id"] == L);
          if (te != null) {
            let se = ye(te, S.refLabel);
            se && ($.value = `${se}`, We.setValue(S.model, L, S.refLabel, se));
          } else {
            const se = q.attributes?.some((C) => C.name == "Computed") == !0;
            let R = await We.getOrFetchValue(v, n.value, S.model, S.refId, S.refLabel, se, L);
            $.value = R || `${S.model}: ${$.value}`;
          }
        }
      }
    }), (Q, q) => {
      const S = U("Icon");
      return a(), i("div", Nd, [
        s("input", {
          type: "hidden",
          name: u.value,
          value: y.value
        }, null, 8, zd),
        d.value ? (a(), i("div", Ud, [
          s("label", {
            for: u.value,
            class: g(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
          }, T(d.value), 11, qd),
          y.value ? (a(), i("div", Qd, [
            s("span", Kd, T(y.value), 1),
            s("button", {
              onClick: D,
              type: "button",
              title: "clear",
              class: "mr-1 rounded-md text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
            }, [...q[1] || (q[1] = [
              s("span", { class: "sr-only" }, "Clear", -1),
              s("svg", {
                class: "h-4 w-4",
                xmlns: "http://www.w3.org/2000/svg",
                fill: "none",
                viewBox: "0 0 24 24",
                "stroke-width": "1.5",
                stroke: "currentColor",
                "aria-hidden": "true"
              }, [
                s("path", {
                  "stroke-linecap": "round",
                  "stroke-linejoin": "round",
                  d: "M6 18L18 6M6 6l12 12"
                })
              ], -1)
            ])])
          ])) : x("", !0)
        ])) : x("", !0),
        M.value ? (a(), i("div", _d, [
          s("button", {
            type: "button",
            class: "lookup flex relative w-full bg-white dark:bg-black border border-gray-300 dark:border-gray-700 rounded-md shadow-sm pl-3 pr-10 py-2 text-left focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm",
            onClick: q[0] || (q[0] = (L) => E(M.value)),
            "aria-haspopup": "listbox",
            "aria-expanded": "true",
            "aria-labelledby": "listbox-label"
          }, [
            s("span", Zd, [
              s("span", Wd, [
                pe(S, {
                  class: "mr-1 w-5 h-5",
                  image: k.value
                }, null, 8, ["image"]),
                s("span", null, T($.value), 1)
              ])
            ]),
            q[2] || (q[2] = s("span", { class: "absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none" }, [
              s("svg", {
                class: "h-5 w-5 text-gray-400 dark:text-gray-500",
                xmlns: "http://www.w3.org/2000/svg",
                viewBox: "0 0 20 20",
                fill: "currentColor",
                "aria-hidden": "true"
              }, [
                s("path", {
                  "fill-rule": "evenodd",
                  d: "M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z",
                  "clip-rule": "evenodd"
                })
              ])
            ], -1))
          ])
        ])) : x("", !0),
        p.value ? (a(), i("p", {
          key: 2,
          class: "mt-2 text-sm text-red-500",
          id: `${u.value}-error`
        }, T(p.value), 9, Gd)) : e.help ? (a(), i("p", {
          key: 3,
          class: "mt-2 text-sm text-gray-500",
          id: `${u.value}-description`
        }, T(e.help), 9, Jd)) : x("", !0)
      ]);
    };
  }
}), Yd = /* @__PURE__ */ ue({
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
    const n = e, o = l;
    t({ forceUpdate: r, props: n, updateValue: d });
    function r() {
      Pe()?.proxy?.$forceUpdate();
    }
    function u(E, D) {
      d(E.id, ye(D, E.id));
    }
    function d(E, D) {
      n.modelValue[E] = D, o("update:modelValue", n.modelValue), r();
    }
    const { metadataApi: f, apiOf: v, typeOf: p, typeOfRef: $, createFormLayout: b, Crud: y } = vt(), h = c(() => n.type || Ht(n.modelValue)), k = c(() => n.metaType ?? p(h.value)), V = c(() => $(f.value?.operations.find((E) => E.request.name == h.value)?.dataModel) || k.value);
    function M() {
      const E = k.value;
      if (!E) {
        if (n.formLayout) {
          const le = n.formLayout.map((ee) => {
            const j = { name: ee.id, type: Qa(ee.type) }, K = Object.assign({ prop: j }, ee);
            return n.configureField && n.configureField(K), K;
          });
          return n.configureFormLayout && n.configureFormLayout(le), le;
        }
        throw new Error(`MetadataType for ${h.value} not found`);
      }
      const D = et(E), Q = V.value, q = n.formLayout ? Array.from(n.formLayout) : b(E), S = [], L = v(E.name);
      return q.forEach((le) => {
        const ee = D.find((_) => _.name == le.name);
        if (le.ignore) return;
        const j = Q?.properties?.find((_) => _.name.toLowerCase() == le.name?.toLowerCase()) ?? ee, K = Object.assign({ prop: j, op: L }, le);
        n.configureField && n.configureField(K), S.push(K);
      }), n.configureFormLayout && n.configureFormLayout(S), S;
    }
    const Y = () => M().filter((E) => E.type != "hidden").map((E) => E.id);
    return (E, D) => {
      const Q = U("ErrorSummary"), q = U("LookupInput"), S = U("DynamicInput");
      return a(), i(Ve, null, [
        e.hideSummary ? x("", !0) : (a(), X(Q, {
          key: 0,
          status: e.api?.error,
          except: Y()
        }, null, 8, ["status", "except"])),
        s("div", {
          class: g(e.flexClass)
        }, [
          s("div", {
            class: g(e.divideClass)
          }, [
            s("div", {
              class: g(e.spaceClass)
            }, [
              s("fieldset", {
                class: g(e.fieldsetClass)
              }, [
                (a(!0), i(Ve, null, Se(M(), (L) => (a(), i("div", {
                  key: L.id,
                  class: g([
                    "w-full",
                    L.css?.field ?? (L.type == "textarea" ? "col-span-12" : "col-span-12 xl:col-span-6" + (L.type == "checkbox" ? " flex items-center" : "")),
                    L.type == "hidden" ? "hidden" : ""
                  ])
                }, [
                  L.type === "lookup" || L.prop?.ref != null && L.type != "file" && !L.prop.isPrimaryKey ? (a(), X(q, {
                    key: 0,
                    metadataType: V.value,
                    input: L,
                    modelValue: e.modelValue,
                    "onUpdate:modelValue": (le) => u(L, le),
                    status: e.api?.error
                  }, null, 8, ["metadataType", "input", "modelValue", "onUpdate:modelValue", "status"])) : (a(), X(S, {
                    key: 1,
                    input: L,
                    modelValue: e.modelValue,
                    "onUpdate:modelValue": D[0] || (D[0] = (le) => E.$emit("update:modelValue", le)),
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
}), ec = { key: 0 }, tc = { class: "text-red-700" }, lc = { key: 0 }, sc = { key: 2 }, nc = ["innerHTML"], ac = { class: "flex justify-end" }, oc = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, ic = { class: "fixed inset-0 overflow-hidden" }, rc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, uc = { class: "flex-1" }, dc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, cc = { class: "flex items-start justify-between space-x-3" }, fc = { class: "space-y-1" }, vc = { key: 0 }, mc = { key: 2 }, gc = ["innerHTML"], pc = { class: "flex h-7 items-center" }, yc = { class: "flex justify-end" }, hc = /* @__PURE__ */ ue({
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
    const n = e, o = l, r = A(), u = A(1), d = A();
    function f() {
      u.value++, _.value = K(), Pe()?.proxy?.$forceUpdate();
    }
    async function v(B) {
      Object.assign(_.value, B), f(), await wt(() => null);
    }
    ul("ModalProvider", {
      openModal: y
    });
    const $ = A(), b = A();
    function y(B, ne) {
      $.value = B, b.value = ne;
    }
    async function h(B) {
      b.value && b.value(B), $.value = void 0, b.value = void 0;
    }
    const k = fl(), { getTypeName: V } = Gn(), { typeOf: M, Crud: Y, createDto: E } = vt(), D = A(new Ge()), Q = c(() => n.panelClass || Oe.panelClass(n.formStyle)), q = c(() => n.formClass || n.formStyle == "card" ? "shadow sm:rounded-md" : Qt.formClass), S = c(() => n.headingClass || Oe.headingClass(n.formStyle)), L = c(() => n.subHeadingClass || Oe.subHeadingClass(n.formStyle)), le = c(() => typeof n.buttonsClass == "string" ? n.buttonsClass : Oe.buttonsClass), ee = c(() => n.type ? V(n.type) : n.modelValue?.getTypeName ? n.modelValue.getTypeName() : null), j = c(() => n.metaType ?? M(ee.value)), K = () => n.modelValue || R(), _ = A(K()), te = c(() => k.loading.value), se = c(() => n.heading != null ? n.heading : j.value?.description || De(ee.value));
    t({ forceUpdate: f, props: n, setModel: v, formFields: r, submit: P, close: J, model: _ });
    function R() {
      return typeof n.type == "string" ? E(n.type) : n.type ? new n.type() : n.modelValue;
    }
    async function C(B) {
      if (!B || B.tagName != "FORM") {
        console.error("Not a valid form", B);
        return;
      }
      const ne = R();
      let ie = Ee(ne?.getMethod, (me) => typeof me == "function" ? me() : null) || "POST", ae = Ee(ne?.createResponse, (me) => typeof me == "function" ? me() : null) == null;
      const oe = n.jsconfig;
      if (ds.hasRequestBody(ie)) {
        let me = new ne.constructor(), ce = new FormData(B);
        ae ? D.value = await k.apiFormVoid(me, ce, { jsconfig: oe }) : D.value = await k.apiForm(me, ce, { jsconfig: oe });
      } else {
        let me = new ne.constructor(Ia(_.value));
        console.debug("AutoForm.submit", me), ae ? D.value = await k.apiVoid(me, { jsconfig: oe }) : D.value = await k.api(me, { jsconfig: oe });
      }
      D.value.succeeded ? (o("success", D.value.response), J()) : o("error", D.value.error);
    }
    async function P() {
      C(d.value);
    }
    function de(B) {
      o("update:modelValue", B);
    }
    function w() {
      o("done");
    }
    const m = A(!1), W = A(""), ve = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    kt(m, () => {
      xt(ve, W, m.value), m.value || setTimeout(w, 700);
    }), m.value = !0;
    function J() {
      n.formStyle == "slideOver" ? m.value = !1 : w();
    }
    const z = (B) => {
      B.key === "Escape" && J();
    };
    return tt(() => window.addEventListener("keydown", z)), jt(() => window.removeEventListener("keydown", z)), (B, ne) => {
      const ie = U("AutoFormFields"), ae = U("FormLoading"), oe = U("PrimaryButton"), me = U("CloseButton"), ce = U("SecondaryButton"), fe = U("ModalLookup");
      return a(), i("div", null, [
        j.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: g(Q.value)
        }, [
          s("form", {
            ref_key: "elForm",
            ref: d,
            onSubmit: ne[0] || (ne[0] = He((Fe) => C(Fe.target), ["prevent"])),
            autocomplete: "off",
            class: g(e.innerFormClass)
          }, [
            s("div", {
              class: g(e.bodyClass)
            }, [
              s("div", {
                class: g(e.headerClass)
              }, [
                B.$slots.heading ? (a(), i("div", lc, [
                  O(B.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: g(S.value)
                }, T(se.value), 3)),
                B.$slots.subheading ? (a(), i("div", sc, [
                  O(B.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: g(L.value)
                }, T(e.subHeading), 3)) : j.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: g(["notes", L.value]),
                  innerHTML: j.value?.notes
                }, null, 10, nc)) : x("", !0)
              ], 2),
              O(B.$slots, "header", {
                instance: Pe()?.exposed,
                model: _.value
              }),
              ne[5] || (ne[5] = s("input", {
                type: "submit",
                class: "hidden"
              }, null, -1)),
              (a(), X(ie, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                type: e.type,
                modelValue: _.value,
                "onUpdate:modelValue": de,
                api: D.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
              O(B.$slots, "footer", {
                instance: Pe()?.exposed,
                model: _.value
              })
            ], 2),
            O(B.$slots, "buttons", {}, () => [
              s("div", {
                class: g(le.value)
              }, [
                s("div", null, [
                  O(B.$slots, "leftbuttons", {
                    instance: Pe()?.exposed,
                    model: _.value
                  })
                ]),
                s("div", null, [
                  e.showLoading && te.value ? (a(), X(ae, { key: 0 })) : x("", !0)
                ]),
                s("div", ac, [
                  ne[6] || (ne[6] = s("div", null, null, -1)),
                  pe(oe, {
                    disabled: te.value || (e.allowSubmit ? !e.allowSubmit(_.value) : !1)
                  }, {
                    default: we(() => [
                      he(T(e.submitLabel), 1)
                    ]),
                    _: 1
                  }, 8, ["disabled"]),
                  O(B.$slots, "rightbuttons", {
                    instance: Pe()?.exposed,
                    model: _.value
                  })
                ])
              ], 2)
            ])
          ], 34)
        ], 2)) : (a(), i("div", oc, [
          ne[8] || (ne[8] = s("div", { class: "fixed inset-0" }, null, -1)),
          s("div", ic, [
            s("div", {
              onMousedown: J,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: ne[2] || (ne[2] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: g(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", W.value])
                }, [
                  s("form", {
                    ref_key: "elForm",
                    ref: d,
                    class: g(q.value),
                    onSubmit: ne[1] || (ne[1] = He((Fe) => C(Fe.target), ["prevent"]))
                  }, [
                    s("div", rc, [
                      s("div", uc, [
                        s("div", dc, [
                          s("div", cc, [
                            s("div", fc, [
                              B.$slots.heading ? (a(), i("div", vc, [
                                O(B.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: g(S.value)
                              }, T(se.value), 3)),
                              B.$slots.subheading ? (a(), i("div", mc, [
                                O(B.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: g(L.value)
                              }, T(e.subHeading), 3)) : j.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: g(["notes", L.value]),
                                innerHTML: j.value?.notes
                              }, null, 10, gc)) : x("", !0)
                            ]),
                            s("div", pc, [
                              pe(me, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: J
                              })
                            ])
                          ])
                        ]),
                        O(B.$slots, "header", {
                          instance: Pe()?.exposed,
                          model: _.value
                        }),
                        (a(), X(ie, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          type: e.type,
                          modelValue: _.value,
                          "onUpdate:modelValue": de,
                          api: D.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
                        O(B.$slots, "footer", {
                          instance: Pe()?.exposed,
                          model: _.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: g(le.value)
                    }, [
                      s("div", null, [
                        O(B.$slots, "leftbuttons", {
                          instance: Pe()?.exposed,
                          model: _.value
                        })
                      ]),
                      s("div", null, [
                        e.showLoading && te.value ? (a(), X(ae, { key: 0 })) : x("", !0)
                      ]),
                      s("div", yc, [
                        pe(ce, {
                          onClick: J,
                          disabled: te.value
                        }, {
                          default: we(() => [...ne[7] || (ne[7] = [
                            he("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"]),
                        pe(oe, {
                          class: "ml-4",
                          disabled: te.value || (e.allowSubmit ? !e.allowSubmit(_.value) : !1)
                        }, {
                          default: we(() => [
                            he(T(e.submitLabel), 1)
                          ]),
                          _: 1
                        }, 8, ["disabled"]),
                        O(B.$slots, "rightbuttons", {
                          instance: Pe()?.exposed,
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
          s("p", tc, [
            ne[3] || (ne[3] = he("Could not create form for unknown ", -1)),
            ne[4] || (ne[4] = s("b", null, "type", -1)),
            he(" " + T(ee.value), 1)
          ])
        ])),
        $.value?.name == "ModalLookup" && $.value.ref ? (a(), X(fe, {
          key: 3,
          "ref-info": $.value.ref,
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
}, Vc = { class: "fixed inset-0 overflow-hidden" }, Mc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, Sc = { class: "flex-1" }, Tc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, Fc = { class: "flex items-start justify-between space-x-3" }, Ac = { class: "space-y-1" }, Ic = { key: 0 }, Pc = { key: 2 }, Dc = ["innerHTML"], Bc = { class: "flex h-7 items-center" }, jc = { class: "flex justify-end" }, Oc = /* @__PURE__ */ ue({
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
    const n = e, o = l, r = A(), u = A(1);
    function d() {
      u.value++, r.value?.forceUpdate(), Pe()?.proxy?.$forceUpdate();
    }
    function f(z) {
      Object.assign(S.value, z), d();
    }
    function v(z) {
    }
    ul("ModalProvider", {
      openModal: y
    });
    const $ = A(), b = A();
    function y(z, B) {
      $.value = z, b.value = B;
    }
    async function h(z) {
      b.value && b.value(z), $.value = void 0, b.value = void 0;
    }
    const { typeOf: k, typeProperties: V, Crud: M, createDto: Y, formValues: E } = vt(), D = c(() => Ht(n.type)), Q = c(() => k(D.value)), S = A(typeof n.type == "string" ? Y(n.type) : n.type ? new n.type() : null);
    t({ forceUpdate: d, props: n, setModel: f, formFields: r, model: S });
    const L = c(() => n.panelClass || Oe.panelClass(n.formStyle)), le = c(() => n.formClass || Oe.formClass(n.formStyle)), ee = c(() => n.headingClass || Oe.headingClass(n.formStyle)), j = c(() => n.subHeadingClass || Oe.subHeadingClass(n.formStyle)), K = c(() => n.buttonsClass || Oe.buttonsClass), _ = c(() => M.model(Q.value)), te = c(() => n.heading || k(D.value)?.description || (_.value ? `New ${De(_.value)}` : De(D.value))), se = A(new Ge());
    let R = fl(), C = c(() => R.loading.value);
    N.interceptors.has("AutoCreateForm.new") && N.interceptors.invoke("AutoCreateForm.new", { props: n, model: S });
    async function P(z) {
      let B = z.target;
      if (!n.autosave) {
        o("save", new S.value.constructor(E(B, V(Q.value))));
        return;
      }
      let ne = Ee(S.value?.getMethod, (ae) => typeof ae == "function" ? ae() : null) || "POST", ie = Ee(S.value?.createResponse, (ae) => typeof ae == "function" ? ae() : null) == null;
      if (ds.hasRequestBody(ne)) {
        let ae = new S.value.constructor(), oe = new FormData(B);
        ie ? se.value = await R.apiFormVoid(ae, oe, { jsconfig: "eccn" }) : se.value = await R.apiForm(ae, oe, { jsconfig: "eccn" });
      } else {
        let ae = E(B, V(Q.value)), oe = new S.value.constructor(ae);
        ie ? se.value = await R.apiVoid(oe, { jsconfig: "eccn" }) : se.value = await R.api(oe, { jsconfig: "eccn" });
      }
      se.value.succeeded ? (B.reset(), o("save", se.value.response)) : o("error", se.value.error);
    }
    function de() {
      o("done");
    }
    const w = A(!1), m = A(""), W = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    kt(w, () => {
      xt(W, m, w.value), w.value || setTimeout(de, 700);
    }), w.value = !0;
    function ve() {
      n.formStyle == "slideOver" ? w.value = !1 : de();
    }
    const J = (z) => {
      z.key === "Escape" && ve();
    };
    return tt(() => window.addEventListener("keydown", J)), jt(() => window.removeEventListener("keydown", J)), (z, B) => {
      const ne = U("AutoFormFields"), ie = U("FormLoading"), ae = U("SecondaryButton"), oe = U("PrimaryButton"), me = U("CloseButton"), ce = U("ModalLookup");
      return a(), i("div", null, [
        Q.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: g(L.value)
        }, [
          s("form", {
            onSubmit: He(P, ["prevent"])
          }, [
            s("div", {
              class: g(le.value)
            }, [
              s("div", null, [
                z.$slots.heading ? (a(), i("div", xc, [
                  O(z.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: g(ee.value)
                }, T(te.value), 3)),
                z.$slots.subheading ? (a(), i("div", kc, [
                  O(z.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: g(j.value)
                }, T(e.subHeading), 3)) : Q.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: g(["notes", j.value]),
                  innerHTML: Q.value?.notes
                }, null, 10, Cc)) : x("", !0)
              ]),
              O(z.$slots, "header", {
                formInstance: Pe()?.exposed,
                model: S.value
              }),
              (a(), X(ne, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                modelValue: S.value,
                "onUpdate:modelValue": v,
                api: se.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              O(z.$slots, "footer", {
                formInstance: Pe()?.exposed,
                model: S.value
              })
            ], 2),
            s("div", {
              class: g(K.value)
            }, [
              s("div", null, [
                e.showLoading && G(C) ? (a(), X(ie, { key: 0 })) : x("", !0)
              ]),
              s("div", $c, [
                e.showCancel ? (a(), X(ae, {
                  key: 0,
                  onClick: ve,
                  disabled: G(C)
                }, {
                  default: we(() => [...B[3] || (B[3] = [
                    he("Cancel", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])) : x("", !0),
                pe(oe, {
                  type: "submit",
                  class: "ml-4",
                  disabled: G(C)
                }, {
                  default: we(() => [...B[4] || (B[4] = [
                    he("Save", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 32)
        ], 2)) : (a(), i("div", Lc, [
          B[7] || (B[7] = s("div", { class: "fixed inset-0" }, null, -1)),
          s("div", Vc, [
            s("div", {
              onMousedown: ve,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: B[0] || (B[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: g(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", m.value])
                }, [
                  s("form", {
                    class: g(le.value),
                    onSubmit: He(P, ["prevent"])
                  }, [
                    s("div", Mc, [
                      s("div", Sc, [
                        s("div", Tc, [
                          s("div", Fc, [
                            s("div", Ac, [
                              z.$slots.heading ? (a(), i("div", Ic, [
                                O(z.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: g(ee.value)
                              }, T(te.value), 3)),
                              z.$slots.subheading ? (a(), i("div", Pc, [
                                O(z.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: g(j.value)
                              }, T(e.subHeading), 3)) : Q.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: g(["notes", j.value]),
                                innerHTML: Q.value?.notes
                              }, null, 10, Dc)) : x("", !0)
                            ]),
                            s("div", Bc, [
                              pe(me, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: ve
                              })
                            ])
                          ])
                        ]),
                        O(z.$slots, "header", {
                          formInstance: Pe()?.exposed,
                          model: S.value
                        }),
                        (a(), X(ne, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          modelValue: S.value,
                          "onUpdate:modelValue": v,
                          api: se.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
                        O(z.$slots, "footer", {
                          formInstance: Pe()?.exposed,
                          model: S.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: g(K.value)
                    }, [
                      s("div", null, [
                        e.showLoading && G(C) ? (a(), X(ie, { key: 0 })) : x("", !0)
                      ]),
                      s("div", jc, [
                        e.showCancel ? (a(), X(ae, {
                          key: 0,
                          onClick: ve,
                          disabled: G(C)
                        }, {
                          default: we(() => [...B[5] || (B[5] = [
                            he("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])) : x("", !0),
                        pe(oe, {
                          type: "submit",
                          class: "ml-4",
                          disabled: G(C)
                        }, {
                          default: we(() => [...B[6] || (B[6] = [
                            he("Save", -1)
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
          s("p", wc, [
            B[1] || (B[1] = he("Could not create form for unknown ", -1)),
            B[2] || (B[2] = s("b", null, "type", -1)),
            he(" " + T(D.value), 1)
          ])
        ])),
        $.value?.name == "ModalLookup" && $.value.ref ? (a(), X(ce, {
          key: 3,
          "ref-info": $.value.ref,
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
}, Qc = { class: "fixed inset-0 overflow-hidden" }, Kc = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, _c = { class: "flex-1" }, Zc = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, Wc = { class: "flex items-start justify-between space-x-3" }, Gc = { class: "space-y-1" }, Jc = { key: 0 }, Xc = { key: 2 }, Yc = ["innerHTML"], e0 = { class: "flex h-7 items-center" }, t0 = { class: "flex justify-end" }, l0 = /* @__PURE__ */ ue({
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
    const n = e, o = l, r = A(), u = A(1);
    function d() {
      u.value++, ee.value = le(), Pe()?.proxy?.$forceUpdate();
    }
    function f(ce) {
      Object.assign(ee.value, ce);
    }
    function v(ce) {
    }
    ul("ModalProvider", {
      openModal: y
    });
    const $ = A(), b = A();
    function y(ce, fe) {
      $.value = ce, b.value = fe;
    }
    async function h(ce) {
      b.value && b.value(ce), $.value = void 0, b.value = void 0;
    }
    const { typeOf: k, apiOf: V, typeProperties: M, createFormLayout: Y, getPrimaryKey: E, Crud: D, createDto: Q, formValues: q } = vt(), S = c(() => Ht(n.type)), L = c(() => k(S.value)), le = () => typeof n.type == "string" ? Q(n.type, sl(n.modelValue)) : n.type ? new n.type(sl(n.modelValue)) : null, ee = A(le());
    t({ forceUpdate: d, props: n, setModel: f, formFields: r, model: ee });
    const j = c(() => n.panelClass || Oe.panelClass(n.formStyle)), K = c(() => n.formClass || Oe.formClass(n.formStyle)), _ = c(() => n.headingClass || Oe.headingClass(n.formStyle)), te = c(() => n.subHeadingClass || Oe.subHeadingClass(n.formStyle)), se = c(() => n.buttonsClass || Oe.buttonsClass), R = c(() => D.model(L.value)), C = c(() => n.heading || k(S.value)?.description || (R.value ? `Update ${De(R.value)}` : De(S.value))), P = A(new Ge());
    let de = Object.assign({}, sl(n.modelValue));
    N.interceptors.has("AutoEditForm.new") && N.interceptors.invoke("AutoEditForm.new", { props: n, model: ee, origModel: de });
    let w = fl(), m = c(() => w.loading.value);
    const W = () => Ee(k(D.model(L.value)), (ce) => E(ce));
    function ve(ce) {
      const { op: fe, prop: Fe } = ce;
      fe && (D.isPatch(fe) || D.isUpdate(fe)) && (ce.disabled = Fe?.isPrimaryKey), n.configureField && n.configureField(ce);
    }
    async function J(ce) {
      let fe = ce.target;
      if (!n.autosave) {
        o("save", new ee.value.constructor(q(fe, M(L.value))));
        return;
      }
      let Fe = Ee(ee.value?.getMethod, ($e) => typeof $e == "function" ? $e() : null) || "POST", je = Ee(ee.value?.createResponse, ($e) => typeof $e == "function" ? $e() : null) == null, xe = W();
      if (ds.hasRequestBody(Fe)) {
        let $e = new ee.value.constructor(), Me = ye(n.modelValue, xe.name), qe = new FormData(fe);
        xe && !Array.from(qe.keys()).some((Qe) => Qe.toLowerCase() == xe.name.toLowerCase()) && qe.append(xe.name, Me);
        let nt = [];
        const $t = S.value && V(S.value);
        if ($t && D.isPatch($t)) {
          let Qe = Y(L.value), lt = {};
          if (xe && (lt[xe.name] = Me), Qe.forEach((ze) => {
            let Xe = ze.id, ot = ye(de, Xe);
            if (xe && xe.name.toLowerCase() === Xe.toLowerCase())
              return;
            let Z = qe.get(Xe);
            N.interceptors.has("AutoEditForm.save.formLayout") && N.interceptors.invoke("AutoEditForm.save.formLayout", { origValue: ot, formLayout: Qe, input: ze, newValue: Z });
            let H = Z != null, Ae = ze.type === "checkbox" ? H !== !!ot : ze.type === "file" ? H : Z != ot;
            !Z && !ot && (Ae = !1), Ae && (Z ? lt[Xe] = Z : ze.type !== "file" && nt.push(Xe));
          }), N.interceptors.has("AutoEditForm.save") && N.interceptors.invoke("AutoEditForm.save", { origModel: de, formLayout: Qe, dirtyValues: lt }), Array.from(qe.keys()).filter((ze) => !lt[ze]).forEach((ze) => qe.delete(ze)), Array.from(qe.keys()).filter((ze) => ze.toLowerCase() != xe.name.toLowerCase()).length == 0 && nt.length == 0) {
            oe();
            return;
          }
        }
        const Lt = nt.length > 0 ? { jsconfig: "eccn", reset: nt } : { jsconfig: "eccn" };
        je ? P.value = await w.apiFormVoid($e, qe, Lt) : P.value = await w.apiForm($e, qe, Lt);
      } else {
        let $e = q(fe, M(L.value));
        xe && !ye($e, xe.name) && ($e[xe.name] = ye(n.modelValue, xe.name));
        let Me = new ee.value.constructor($e);
        je ? P.value = await w.apiVoid(Me, { jsconfig: "eccn" }) : P.value = await w.api(Me, { jsconfig: "eccn" });
      }
      P.value.succeeded ? (fe.reset(), o("save", P.value.response)) : o("error", P.value.error);
    }
    async function z(ce) {
      let fe = W();
      const Fe = fe ? ye(n.modelValue, fe.name) : null;
      if (!Fe) {
        console.error(`Could not find Primary Key for Type ${S.value} (${R.value})`);
        return;
      }
      const je = { [fe.name]: Fe }, xe = typeof n.deleteType == "string" ? Q(n.deleteType, je) : n.deleteType ? new n.deleteType(je) : null;
      Ee(xe.createResponse, (Me) => typeof Me == "function" ? Me() : null) == null ? P.value = await w.apiVoid(xe) : P.value = await w.api(xe), P.value.succeeded ? o("delete", P.value.response) : o("error", P.value.error);
    }
    function B() {
      o("done");
    }
    const ne = A(!1), ie = A(""), ae = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    kt(ne, () => {
      xt(ae, ie, ne.value), ne.value || setTimeout(B, 700);
    }), ne.value = !0;
    function oe() {
      n.formStyle == "slideOver" ? ne.value = !1 : B();
    }
    const me = (ce) => {
      ce.key === "Escape" && oe();
    };
    return tt(() => window.addEventListener("keydown", me)), jt(() => window.removeEventListener("keydown", me)), (ce, fe) => {
      const Fe = U("AutoFormFields"), je = U("ConfirmDelete"), xe = U("FormLoading"), $e = U("SecondaryButton"), Me = U("PrimaryButton"), qe = U("CloseButton"), nt = U("ModalLookup");
      return a(), i("div", null, [
        L.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: g(j.value)
        }, [
          s("form", {
            onSubmit: He(J, ["prevent"])
          }, [
            s("div", {
              class: g(K.value)
            }, [
              s("div", null, [
                ce.$slots.heading ? (a(), i("div", Ec, [
                  O(ce.$slots, "heading")
                ])) : (a(), i("h3", {
                  key: 1,
                  class: g(_.value)
                }, T(C.value), 3)),
                ce.$slots.subheading ? (a(), i("div", Nc, [
                  O(ce.$slots, "subheading")
                ])) : e.subHeading ? (a(), i("p", {
                  key: 3,
                  class: g(te.value)
                }, T(e.subHeading), 3)) : L.value?.notes ? (a(), i("p", {
                  key: 4,
                  class: g(["notes", te.value]),
                  innerHTML: L.value?.notes
                }, null, 10, zc)) : x("", !0)
              ]),
              O(ce.$slots, "header", {
                formInstance: Pe()?.exposed,
                model: ee.value
              }),
              (a(), X(Fe, {
                ref_key: "formFields",
                ref: r,
                key: u.value,
                modelValue: ee.value,
                "onUpdate:modelValue": v,
                api: P.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              O(ce.$slots, "footer", {
                formInstance: Pe()?.exposed,
                model: ee.value
              })
            ], 2),
            s("div", {
              class: g(se.value)
            }, [
              s("div", null, [
                e.deleteType ? (a(), X(je, {
                  key: 0,
                  onDelete: z
                })) : x("", !0)
              ]),
              s("div", null, [
                e.showLoading && G(m) ? (a(), X(xe, { key: 0 })) : x("", !0)
              ]),
              s("div", Uc, [
                e.showCancel ? (a(), X($e, {
                  key: 0,
                  onClick: oe,
                  disabled: G(m)
                }, {
                  default: we(() => [...fe[3] || (fe[3] = [
                    he("Cancel", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])) : x("", !0),
                pe(Me, {
                  type: "submit",
                  class: "ml-4",
                  disabled: G(m)
                }, {
                  default: we(() => [...fe[4] || (fe[4] = [
                    he("Save", -1)
                  ])]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 32)
        ], 2)) : (a(), i("div", qc, [
          fe[7] || (fe[7] = s("div", { class: "fixed inset-0" }, null, -1)),
          s("div", Qc, [
            s("div", {
              onMousedown: oe,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: fe[0] || (fe[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: g(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", ie.value])
                }, [
                  s("form", {
                    class: g(K.value),
                    onSubmit: He(J, ["prevent"])
                  }, [
                    s("div", Kc, [
                      s("div", _c, [
                        s("div", Zc, [
                          s("div", Wc, [
                            s("div", Gc, [
                              ce.$slots.heading ? (a(), i("div", Jc, [
                                O(ce.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: g(_.value)
                              }, T(C.value), 3)),
                              ce.$slots.subheading ? (a(), i("div", Xc, [
                                O(ce.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: g(te.value)
                              }, T(e.subHeading), 3)) : L.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: g(["notes", te.value]),
                                innerHTML: L.value?.notes
                              }, null, 10, Yc)) : x("", !0)
                            ]),
                            s("div", e0, [
                              pe(qe, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: oe
                              })
                            ])
                          ])
                        ]),
                        O(ce.$slots, "header", {
                          formInstance: Pe()?.exposed,
                          model: ee.value
                        }),
                        (a(), X(Fe, {
                          ref_key: "formFields",
                          ref: r,
                          key: u.value,
                          modelValue: ee.value,
                          "onUpdate:modelValue": v,
                          api: P.value,
                          configureField: ve,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureFormLayout"])),
                        O(ce.$slots, "footer", {
                          formInstance: Pe()?.exposed,
                          model: ee.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: g(se.value)
                    }, [
                      s("div", null, [
                        e.deleteType ? (a(), X(je, {
                          key: 0,
                          onDelete: z
                        })) : x("", !0)
                      ]),
                      s("div", null, [
                        e.showLoading && G(m) ? (a(), X(xe, { key: 0 })) : x("", !0)
                      ]),
                      s("div", t0, [
                        e.showCancel ? (a(), X($e, {
                          key: 0,
                          onClick: oe,
                          disabled: G(m)
                        }, {
                          default: we(() => [...fe[5] || (fe[5] = [
                            he("Cancel", -1)
                          ])]),
                          _: 1
                        }, 8, ["disabled"])) : x("", !0),
                        pe(Me, {
                          type: "submit",
                          class: "ml-4",
                          disabled: G(m)
                        }, {
                          default: we(() => [...fe[6] || (fe[6] = [
                            he("Save", -1)
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
          s("p", Rc, [
            fe[1] || (fe[1] = he("Could not create form for unknown ", -1)),
            fe[2] || (fe[2] = s("b", null, "type", -1)),
            he(" " + T(S.value), 1)
          ])
        ])),
        $.value?.name == "ModalLookup" && $.value.ref ? (a(), X(nt, {
          key: 3,
          "ref-info": $.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ]);
    };
  }
}), s0 = { key: 0 }, n0 = { class: "text-red-700" }, a0 = { key: 0 }, o0 = { key: 2 }, i0 = ["innerHTML"], r0 = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, u0 = { class: "fixed inset-0 overflow-hidden" }, d0 = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, c0 = { class: "flex-1" }, f0 = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, v0 = { class: "flex items-start justify-between space-x-3" }, m0 = { class: "space-y-1" }, g0 = { key: 0 }, p0 = { key: 2 }, y0 = ["innerHTML"], h0 = { class: "flex h-7 items-center" }, b0 = /* @__PURE__ */ ue({
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
    const l = e, n = t, { typeOf: o, getPrimaryKey: r, Crud: u, createDto: d } = vt(), f = c(() => l.typeName ?? l.apis.dataModel.name), v = c(() => o(f.value)), p = c(() => l.panelClass || Oe.panelClass(l.formStyle)), $ = c(() => l.formClass || Oe.formClass(l.formStyle)), b = c(() => l.headingClass || Oe.headingClass(l.formStyle)), y = c(() => l.subHeadingClass || Oe.subHeadingClass(l.formStyle)), h = c(() => l.heading || o(f.value)?.description || (l.model?.id ? `${De(f.value)} ${l.model.id}` : "View " + De(f.value))), k = A(new Ge());
    Object.assign({}, sl(l.model)), N.interceptors.has("AutoViewForm.new") && N.interceptors.invoke("AutoViewForm.new", { props: l });
    let V = fl(), M = c(() => V.loading.value);
    const Y = () => Ee(v.value, (j) => r(j)), E = c(() => v.value);
    async function D(j) {
      let K = Y();
      const _ = K ? ye(l.model, K.name) : null;
      if (!_) {
        console.error(`Could not find Primary Key for Type ${f.value} (${E.value})`);
        return;
      }
      const te = { [K.name]: _ }, se = typeof l.deleteType == "string" ? d(l.deleteType, te) : l.deleteType ? new l.deleteType(te) : null;
      Ee(se.createResponse, (C) => typeof C == "function" ? C() : null) == null ? k.value = await V.apiVoid(se) : k.value = await V.api(se), k.value.succeeded ? n("delete", k.value.response) : n("error", k.value.error);
    }
    function Q() {
      l.done && l.done();
    }
    const q = A(!1), S = A(""), L = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    kt(q, () => {
      xt(L, S, q.value), q.value || setTimeout(Q, 700);
    }), q.value = !0;
    function le() {
      l.formStyle == "slideOver" ? q.value = !1 : Q();
    }
    const ee = (j) => {
      j.key === "Escape" && le();
    };
    return tt(() => window.addEventListener("keydown", ee)), jt(() => window.removeEventListener("keydown", ee)), (j, K) => {
      const _ = U("MarkupModel"), te = U("CloseButton"), se = U("ConfirmDelete"), R = U("FormLoading");
      return a(), i("div", null, [
        f.value ? e.formStyle == "card" ? (a(), i("div", {
          key: 1,
          class: g(p.value)
        }, [
          s("div", {
            class: g($.value)
          }, [
            s("div", null, [
              j.$slots.heading ? (a(), i("div", a0, [
                O(j.$slots, "heading")
              ])) : (a(), i("h3", {
                key: 1,
                class: g(b.value)
              }, T(h.value), 3)),
              j.$slots.subheading ? (a(), i("div", o0, [
                O(j.$slots, "subheading")
              ])) : e.subHeading ? (a(), i("p", {
                key: 3,
                class: g(y.value)
              }, T(e.subHeading), 3)) : v.value?.notes ? (a(), i("p", {
                key: 4,
                class: g(["notes", y.value]),
                innerHTML: v.value?.notes
              }, null, 10, i0)) : x("", !0)
            ]),
            pe(_, { value: e.model }, null, 8, ["value"])
          ], 2)
        ], 2)) : (a(), i("div", r0, [
          K[4] || (K[4] = s("div", { class: "fixed inset-0" }, null, -1)),
          s("div", u0, [
            s("div", {
              onMousedown: le,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: K[0] || (K[0] = He(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: g(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", S.value])
                }, [
                  s("div", {
                    class: g($.value)
                  }, [
                    s("div", d0, [
                      s("div", c0, [
                        s("div", f0, [
                          s("div", v0, [
                            s("div", m0, [
                              j.$slots.heading ? (a(), i("div", g0, [
                                O(j.$slots, "heading")
                              ])) : (a(), i("h3", {
                                key: 1,
                                class: g(b.value)
                              }, T(h.value), 3)),
                              j.$slots.subheading ? (a(), i("div", p0, [
                                O(j.$slots, "subheading")
                              ])) : e.subHeading ? (a(), i("p", {
                                key: 3,
                                class: g(y.value)
                              }, T(e.subHeading), 3)) : v.value?.notes ? (a(), i("p", {
                                key: 4,
                                class: g(["notes", y.value]),
                                innerHTML: v.value?.notes
                              }, null, 10, y0)) : x("", !0)
                            ]),
                            s("div", h0, [
                              pe(te, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: le
                              })
                            ])
                          ])
                        ]),
                        s("div", null, [
                          pe(_, {
                            value: e.model,
                            tableClass: "w-full border-separate border-spacing-y-1",
                            basicTrClass: "group",
                            basicThClass: "py-1 px-4 text-left text-sm font-medium text-gray-700 dark:text-gray-300",
                            basicTdClass: "py-1 px-4 text-sm text-gray-900",
                            complexTitleTrClass: "group",
                            complexTitleTdClass: "py-1 px-4 font-semibold bg-indigo-600 dark:bg-indigo-700 text-white",
                            complexBodyTrClass: "group",
                            complexBodyTdClass: "py-1 px-4 bg-white dark:bg-gray-900"
                          }, null, 8, ["value"])
                        ])
                      ])
                    ]),
                    s("div", {
                      class: g(G(Oe).buttonsClass)
                    }, [
                      s("div", null, [
                        e.deleteType ? (a(), X(se, {
                          key: 0,
                          onDelete: D
                        })) : x("", !0)
                      ]),
                      s("div", null, [
                        e.showLoading && G(M) ? (a(), X(R, { key: 0 })) : x("", !0)
                      ]),
                      K[3] || (K[3] = s("div", { class: "flex justify-end" }, null, -1))
                    ], 2)
                  ], 2)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), i("div", s0, [
          s("p", n0, [
            K[1] || (K[1] = he("Could not create view for unknown ", -1)),
            K[2] || (K[2] = s("b", null, "type", -1)),
            he(" " + T(f.value), 1)
          ])
        ]))
      ]);
    };
  }
}), w0 = /* @__PURE__ */ ue({
  __name: "ConfirmDelete",
  emits: ["delete"],
  setup(e, { emit: t }) {
    let l = A(!1);
    const n = t, o = () => {
      l.value && n("delete");
    }, r = c(() => [
      "select-none inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white",
      l.value ? "cursor-pointer bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500" : "bg-red-400"
    ]);
    return (u, d) => (a(), i(Ve, null, [
      Pt(s("input", {
        id: "confirmDelete",
        type: "checkbox",
        class: "focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800 dark:ring-offset-black",
        "onUpdate:modelValue": d[0] || (d[0] = (f) => en(l) ? l.value = f : l = f)
      }, null, 512), [
        [rs, G(l)]
      ]),
      d[2] || (d[2] = s("label", {
        for: "confirmDelete",
        class: "ml-2 mr-2 select-none"
      }, "confirm", -1)),
      s("span", Ce({
        onClick: He(o, ["prevent"]),
        class: r.value
      }, u.$attrs), [
        O(u.$slots, "default", {}, () => [
          d[1] || (d[1] = he("Delete", -1))
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
}, C0 = { class: "ml-2 mt-1 text-gray-400" }, $0 = /* @__PURE__ */ ue({
  __name: "FormLoading",
  props: {
    icon: { type: Boolean, default: !0 },
    text: { default: "loading..." }
  },
  setup(e) {
    return Ne("ApiState", void 0), (t, l) => (a(), i("div", x0, [
      e.icon ? (a(), i("svg", k0, [...l[0] || (l[0] = [
        Sl('<rect x="0" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="8" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="16" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate></rect>', 3)
      ])])) : x("", !0),
      s("span", C0, T(e.text), 1)
    ]));
  }
}), L0 = ["onClick"], V0 = {
  key: 3,
  class: "flex justify-between items-center"
}, M0 = { class: "mr-1 select-none" }, S0 = ["onClick"], T0 = /* @__PURE__ */ ue({
  inheritAttrs: !1,
  __name: "DataGrid",
  props: {
    items: { default: () => [] },
    id: { default: "DataGrid" },
    ctx: {},
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
    const l = e, n = t, o = A(), r = A(null), u = (w) => r.value === w, d = Tl(), f = Object.keys(d), v = f.map((w) => w.toLowerCase()), p = (w) => v.includes(w.toLowerCase()), $ = (w) => f.find((m) => m.toLowerCase() == w.toLowerCase() + "-header"), b = (w) => f.find((m) => m.toLowerCase() == w.toLowerCase()), y = c(() => Jl(l.items).filter((w) => p(w) || p(w + "-header"))), h = c(() => l.ctx?.dataModelName || Ht(l.type)), k = c(() => l.ctx?.dataModel || Je(h.value)), V = c(() => l.ctx?.dataModelProps || et(k.value));
    function M(w) {
      const m = l.headerTitles && ye(l.headerTitles, w) || w;
      return l.headerTitle ? l.headerTitle(m) : rn(m);
    }
    function Y(w) {
      const m = w.toLowerCase();
      return V.value.find((W) => W.name.toLowerCase() == m);
    }
    function E(w) {
      const m = Y(w);
      return m?.format ? m.format : m?.type == "TimeSpan" || m?.type == "TimeOnly" ? { method: "time" } : null;
    }
    const D = {
      xs: "xs:table-cell",
      sm: "sm:table-cell",
      md: "md:table-cell",
      lg: "lg:table-cell",
      xl: "xl:table-cell",
      "2xl": "2xl:table-cell",
      never: ""
    };
    function Q(w) {
      const m = l.visibleFrom && ye(l.visibleFrom, w);
      return m && Ee(D[m], (W) => `hidden ${W}`);
    }
    const q = c(() => l.gridClass ?? ge.getGridClass(l.tableStyle)), S = c(() => l.grid2Class ?? ge.getGrid2Class(l.tableStyle)), L = c(() => l.grid3Class ?? ge.getGrid3Class(l.tableStyle)), le = c(() => l.grid4Class ?? ge.getGrid4Class(l.tableStyle)), ee = c(() => l.tableClass ?? ge.getTableClass(l.tableStyle)), j = c(() => l.tbodyClass ?? ge.getTbodyClass(l.tbodyClass)), K = c(() => l.theadClass ?? ge.getTheadClass(l.tableStyle)), _ = c(() => l.theadRowClass ?? ge.getTheadRowClass(l.tableStyle)), te = c(() => l.theadCellClass ?? ge.getTheadCellClass(l.tableStyle));
    function se(w, m) {
      return l.rowClass ? l.rowClass(w, m) : ge.getTableRowClass(l.tableStyle, m, !!(l.isSelected && l.isSelected(w)), l.isSelected != null);
    }
    function R(w, m) {
      return l.rowStyle ? l.rowStyle(w, m) : void 0;
    }
    const C = c(() => {
      const w = (typeof l.selectedColumns == "string" ? l.selectedColumns.split(",") : l.selectedColumns) || (y.value.length > 0 ? y.value : Jl(l.items)), m = V.value.reduce((W, ve) => (W[ve.name.toLowerCase()] = ve.format, W), {});
      return w.filter((W) => m[W.toLowerCase()]?.method != "hidden");
    });
    function P(w, m) {
      n("headerSelected", m, w);
    }
    function de(w, m, W) {
      n("rowSelected", W, w);
    }
    return (w, m) => {
      const W = U("CellFormat"), ve = U("PreviewFormat");
      return e.items.length ? (a(), i("div", {
        key: 0,
        ref_key: "refResults",
        ref: o,
        class: g(q.value)
      }, [
        s("div", {
          class: g(S.value)
        }, [
          s("div", {
            class: g(L.value)
          }, [
            s("div", {
              class: g(le.value)
            }, [
              s("table", {
                class: g(ee.value)
              }, [
                s("thead", {
                  class: g(K.value)
                }, [
                  s("tr", {
                    class: g(_.value)
                  }, [
                    (a(!0), i(Ve, null, Se(C.value, (J) => (a(), i("td", {
                      class: g([Q(J), te.value, u(J) ? "text-gray-900 dark:text-gray-50" : "text-gray-500 dark:text-gray-400"])
                    }, [
                      s("div", {
                        onClick: (z) => P(z, J)
                      }, [
                        G(d)[J + "-header"] ? O(w.$slots, J + "-header", {
                          key: 0,
                          column: J
                        }) : $(J) ? O(w.$slots, $(J), {
                          key: 1,
                          column: J
                        }) : G(d).header ? O(w.$slots, "header", {
                          key: 2,
                          column: J,
                          label: M(J)
                        }) : (a(), i("div", V0, [
                          s("span", M0, T(M(J)), 1)
                        ]))
                      ], 8, L0)
                    ], 2))), 256))
                  ], 2)
                ], 2),
                s("tbody", {
                  class: g(j.value)
                }, [
                  (a(!0), i(Ve, null, Se(e.items, (J, z) => (a(), i("tr", {
                    class: g(se(J, z)),
                    style: is(R(J, z)),
                    onClick: (B) => de(B, z, J)
                  }, [
                    (a(!0), i(Ve, null, Se(C.value, (B) => (a(), i("td", {
                      class: g([Q(B), G(ge).tableCellClass])
                    }, [
                      G(d)[B] ? O(w.$slots, B, Ce({
                        key: 0,
                        ref_for: !0
                      }, J)) : b(B) ? O(w.$slots, b(B), Ce({
                        key: 1,
                        ref_for: !0
                      }, J)) : Y(B) ? (a(), X(W, {
                        key: 2,
                        type: k.value,
                        propType: Y(B),
                        modelValue: J
                      }, null, 8, ["type", "propType", "modelValue"])) : (a(), X(ve, {
                        key: 3,
                        value: G(ye)(J, B),
                        format: E(B),
                        modelValue: J
                      }, null, 8, ["value", "format", "modelValue"]))
                    ], 2))), 256))
                  ], 14, S0))), 256))
                ], 2)
              ], 2)
            ], 2)
          ], 2)
        ], 2)
      ], 2)) : x("", !0);
    };
  }
}), F0 = ue({
  props: {
    type: Object,
    propType: Object,
    modelValue: Object
  },
  setup(e, { attrs: t }) {
    const { typeOf: l } = vt();
    function n(o) {
      return o?.format ? o.format : o?.type == "TimeSpan" || o?.type == "TimeOnly" ? { method: "time" } : null;
    }
    return () => {
      const o = n(e.propType), r = ye(e.modelValue, e.propType.name), u = Object.assign({}, e, t), d = St("span", { innerHTML: il(r, o, u) }), f = rl(r) && Array.isArray(r) ? St("span", {}, [
        St("span", { class: "mr-2" }, `${r.length}`),
        d
      ]) : d, v = e.propType?.ref;
      if (!v)
        return f;
      const $ = et(e.type).find((M) => M.type === v.model);
      if (!$)
        return f;
      const b = ye(e.modelValue, $.name), y = b && v.refLabel && ye(b, v.refLabel);
      if (!y)
        return f;
      const k = l(v.model)?.icon, V = k ? St(Yn, { image: k, class: "w-5 h-5 mr-1" }) : null;
      return St("span", { class: "flex", title: `${v.model} ${r}` }, [
        V,
        y
      ]);
    };
  }
}), A0 = { key: 0 }, I0 = {
  key: 0,
  class: "mr-2"
}, P0 = ["innerHTML"], D0 = ["innerHTML"], B0 = {
  inheritAttrs: !1
}, j0 = /* @__PURE__ */ ue({
  ...B0,
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
    return (n, o) => G(rl)(e.value) ? (a(), i("span", A0, [
      e.includeCount && l.value ? (a(), i("span", I0, T(e.value.length), 1)) : x("", !0),
      s("span", {
        innerHTML: G(il)(e.value, e.format, n.$attrs)
      }, null, 8, P0)
    ])) : (a(), i("span", {
      key: 1,
      innerHTML: G(il)(e.value, e.format, n.$attrs)
    }, null, 8, D0));
  }
}), O0 = ["innerHTML"], H0 = { key: 0 }, R0 = { key: 2 }, E0 = /* @__PURE__ */ ue({
  __name: "HtmlFormat",
  props: {
    value: {},
    depth: { default: 0 },
    fieldAttrs: {},
    classes: { type: Function, default: (e, t, l, n, o) => n }
  },
  setup(e) {
    const t = e, l = c(() => Bt(t.value)), n = c(() => Array.isArray(t.value)), o = (f) => rn(f), r = (f) => t.fieldAttrs ? t.fieldAttrs(f) : null, u = c(() => Jl(t.value)), d = (f) => f ? Object.keys(f).map((v) => ({ key: o(v), val: f[v] })) : [];
    return (f, v) => {
      const p = U("HtmlFormat", !0);
      return a(), i("div", {
        class: g(e.depth == 0 ? "prose html-format" : "")
      }, [
        l.value ? (a(), i("div", {
          key: 0,
          innerHTML: G(il)(e.value)
        }, null, 8, O0)) : n.value ? (a(), i("div", {
          key: 1,
          class: g(e.classes("array", "div", e.depth, G(ge).gridClass))
        }, [
          G(Bt)(e.value[0]) ? (a(), i("div", H0, "[ " + T(e.value.join(", ")) + " ]", 1)) : (a(), i("div", {
            key: 1,
            class: g(e.classes("array", "div", e.depth, G(ge).grid2Class))
          }, [
            s("div", {
              class: g(e.classes("array", "div", e.depth, G(ge).grid3Class))
            }, [
              s("div", {
                class: g(e.classes("array", "div", e.depth, G(ge).grid4Class))
              }, [
                s("table", {
                  class: g(e.classes("object", "table", e.depth, G(ge).tableClass))
                }, [
                  s("thead", {
                    class: g(e.classes("array", "thead", e.depth, G(ge).theadClass))
                  }, [
                    s("tr", null, [
                      (a(!0), i(Ve, null, Se(u.value, ($) => (a(), i("th", {
                        class: g(e.classes("array", "th", e.depth, G(ge).theadCellClass + " whitespace-nowrap"))
                      }, [
                        v[0] || (v[0] = s("b", null, null, -1)),
                        he(T(o($)), 1)
                      ], 2))), 256))
                    ])
                  ], 2),
                  s("tbody", null, [
                    (a(!0), i(Ve, null, Se(e.value, ($, b) => (a(), i("tr", {
                      class: g(e.classes("array", "tr", e.depth, b % 2 == 0 ? "bg-white" : "bg-gray-50", b))
                    }, [
                      (a(!0), i(Ve, null, Se(u.value, (y) => (a(), i("td", {
                        class: g(e.classes("array", "td", e.depth, G(ge).tableCellClass))
                      }, [
                        pe(p, Ce({
                          value: $[y],
                          "field-attrs": e.fieldAttrs,
                          depth: e.depth + 1,
                          classes: e.classes
                        }, { ref_for: !0 }, r(y)), null, 16, ["value", "field-attrs", "depth", "classes"])
                      ], 2))), 256))
                    ], 2))), 256))
                  ])
                ], 2)
              ], 2)
            ], 2)
          ], 2))
        ], 2)) : (a(), i("div", R0, [
          s("table", {
            class: g(e.classes("object", "table", e.depth, "table-object"))
          }, [
            (a(!0), i(Ve, null, Se(d(e.value), ($) => (a(), i("tr", {
              class: g(e.classes("object", "tr", e.depth, ""))
            }, [
              s("th", {
                class: g(e.classes("object", "th", e.depth, "align-top py-2 px-4 text-left text-sm font-medium tracking-wider whitespace-nowrap"))
              }, T($.key), 3),
              s("td", {
                class: g(e.classes("object", "td", e.depth, "align-top py-2 px-4 text-sm"))
              }, [
                pe(p, Ce({
                  value: $.val,
                  "field-attrs": e.fieldAttrs,
                  depth: e.depth + 1,
                  classes: e.classes
                }, { ref_for: !0 }, r($.key)), null, 16, ["value", "field-attrs", "depth", "classes"])
              ], 2)
            ], 2))), 256))
          ], 2)
        ]))
      ], 2);
    };
  }
}), N0 = ["href"], z0 = ["href", "title"], U0 = /* @__PURE__ */ ue({
  __name: "MarkupFormat",
  props: {
    value: {},
    imageClass: { default: "w-8 h-8" }
  },
  setup(e) {
    const t = e, { getMimeType: l } = Ha(), n = t.value;
    let o = typeof t.value;
    const r = o === "string" && n.length ? l(n) : null;
    if (o === "string" && n.length) {
      const u = n.startsWith("https://") || n.startsWith("http://");
      (u || n[0] === "/") && r?.startsWith("image/") ? o = "image" : u && (o = "link");
    }
    return (u, d) => {
      const f = U("Icon"), v = U("HtmlFormat");
      return G(o) == "link" ? (a(), i("a", {
        key: 0,
        href: e.value,
        class: "text-indigo-600"
      }, T(e.value), 9, N0)) : G(o) == "image" ? (a(), i("a", {
        key: 1,
        href: e.value,
        title: e.value,
        class: "inline-block"
      }, [
        pe(f, {
          src: e.value,
          class: g(e.imageClass)
        }, null, 8, ["src", "class"])
      ], 8, z0)) : (a(), X(v, {
        key: 2,
        value: e.value
      }, null, 8, ["value"]));
    };
  }
}), q0 = /* @__PURE__ */ ue({
  __name: "MarkupModel",
  props: {
    value: {},
    imageClass: {},
    tableClass: {},
    basicTrClass: {},
    basicThClass: {},
    basicTdClass: {},
    complexTitleTrClass: {},
    complexTitleTdClass: {},
    complexBodyTrClass: {},
    complexBodyTdClass: {}
  },
  setup(e) {
    const t = e, l = Object.keys(t.value), n = {}, o = {};
    return l.forEach((r) => {
      const u = t.value[r], d = typeof u;
      u == null || d === "function" || d === "symbol" ? n[r] = `(${u == null ? "null" : "t"})` : d === "object" ? o[r] = u : n[r] = u;
    }), (r, u) => {
      const d = U("MarkupFormat");
      return a(), i("table", {
        class: g(t.tableClass ?? "my-2 w-full")
      }, [
        s("tbody", null, [
          (a(), i(Ve, null, Se(n, (f, v) => s("tr", {
            class: g(t.basicTrClass ?? "leading-7")
          }, [
            s("th", {
              class: g(t.basicThClass ?? "px-2 text-left align-top")
            }, T(G(De)(v)), 3),
            s("td", {
              class: g(t.basicTdClass ?? "align-top")
            }, [
              pe(d, { value: f }, null, 8, ["value"])
            ], 2)
          ], 2)), 64)),
          (a(), i(Ve, null, Se(o, (f, v) => (a(), i(Ve, null, [
            s("tr", {
              class: g(t.complexTitleTrClass ?? "my-2 leading-7")
            }, [
              s("th", {
                colspan: "2",
                class: g(t.complexTitleTdClass ?? "px-2 bg-indigo-700 text-white")
              }, T(G(De)(v)), 3)
            ], 2),
            s("tr", {
              class: g(t.complexBodyTrClass ?? "leading-7")
            }, [
              s("td", {
                colspan: "2",
                class: g(t.complexBodyTdClass ?? "px-2 align-top")
              }, [
                pe(d, { value: f }, null, 8, ["value"])
              ], 2)
            ], 2)
          ], 64))), 64))
        ])
      ], 2);
    };
  }
}), Q0 = { class: "absolute top-0 right-0 pt-4 pr-4" }, K0 = ["title"], _0 = /* @__PURE__ */ ue({
  __name: "CloseButton",
  props: {
    buttonClass: { default: "bg-white dark:bg-black" },
    title: { default: "Close" }
  },
  emits: ["close"],
  setup(e, { emit: t }) {
    return (l, n) => (a(), i("div", Q0, [
      s("button", {
        type: "button",
        onClick: n[0] || (n[0] = (o) => l.$emit("close")),
        title: e.title,
        class: g([e.buttonClass, "cursor-pointer rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"])
      }, [...n[1] || (n[1] = [
        s("span", { class: "sr-only" }, "Close", -1),
        s("svg", {
          class: "h-6 w-6",
          xmlns: "http://www.w3.org/2000/svg",
          fill: "none",
          viewBox: "0 0 24 24",
          stroke: "currentColor",
          "aria-hidden": "true"
        }, [
          s("path", {
            "stroke-linecap": "round",
            "stroke-linejoin": "round",
            "stroke-width": "2",
            d: "M6 18L18 6M6 6l12 12"
          })
        ], -1)
      ])], 10, K0)
    ]));
  }
}), Z0 = ["id", "aria-labelledby"], W0 = { class: "fixed inset-0 overflow-hidden" }, G0 = { class: "flex h-full flex-col bg-white dark:bg-black shadow-xl" }, J0 = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, X0 = { class: "flex-1" }, Y0 = { class: "relative bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, ef = { class: "flex items-start justify-between space-x-3" }, tf = { class: "space-y-1" }, lf = { key: 0 }, sf = ["id"], nf = {
  key: 2,
  class: "text-sm text-gray-500"
}, af = { class: "flex h-7 items-center" }, of = {
  key: 0,
  class: "flex-shrink-0 border-t border-gray-200 dark:border-gray-700 px-4 py-5 sm:px-6"
}, rf = /* @__PURE__ */ ue({
  __name: "SlideOver",
  props: {
    id: { default: "SlideOver" },
    title: {},
    contentClass: { default: "relative mt-6 flex-1 px-4 sm:px-6" }
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = t, n = A(!1), o = A(""), r = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    kt(n, () => {
      xt(r, o, n.value), n.value || setTimeout(() => l("done"), 700);
    }), n.value = !0;
    const u = () => n.value = !1, d = (f) => {
      f.key === "Escape" && u();
    };
    return tt(() => window.addEventListener("keydown", d)), jt(() => window.removeEventListener("keydown", d)), (f, v) => {
      const p = U("CloseButton");
      return a(), i("div", {
        id: e.id,
        class: "relative z-10",
        "aria-labelledby": e.id + "-title",
        role: "dialog",
        "aria-modal": "true"
      }, [
        v[1] || (v[1] = s("div", { class: "fixed inset-0" }, null, -1)),
        s("div", W0, [
          s("div", {
            onMousedown: u,
            class: "absolute inset-0 overflow-hidden"
          }, [
            s("div", {
              onMousedown: v[0] || (v[0] = He(() => {
              }, ["stop"])),
              class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
            }, [
              s("div", {
                class: g(["panel pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", o.value])
              }, [
                s("div", G0, [
                  s("div", J0, [
                    s("div", X0, [
                      s("div", Y0, [
                        s("div", ef, [
                          s("div", tf, [
                            f.$slots.title ? (a(), i("div", lf, [
                              O(f.$slots, "title")
                            ])) : x("", !0),
                            e.title ? (a(), i("h2", {
                              key: 1,
                              class: "text-lg font-medium text-gray-900 dark:text-gray-50",
                              id: e.id + "-title"
                            }, T(e.title), 9, sf)) : x("", !0),
                            f.$slots.subtitle ? (a(), i("p", nf, [
                              O(f.$slots, "subtitle")
                            ])) : x("", !0)
                          ]),
                          s("div", af, [
                            pe(p, {
                              "button-class": "bg-gray-50 dark:bg-gray-900",
                              onClose: u
                            })
                          ])
                        ])
                      ]),
                      s("div", {
                        class: g(e.contentClass)
                      }, [
                        O(f.$slots, "default")
                      ], 2)
                    ])
                  ]),
                  f.$slots.footer ? (a(), i("div", of, [
                    O(f.$slots, "footer")
                  ])) : x("", !0)
                ])
              ], 2)
            ], 32)
          ], 32)
        ])
      ], 8, Z0);
    };
  }
}), uf = ["id", "data-transition-for", "aria-labelledby"], df = { class: "fixed inset-0 z-10 overflow-y-auto" }, cf = { class: "flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0" }, ff = {
  key: 1,
  class: "hidden sm:block absolute top-0 right-0 pt-4 pr-4 z-10"
}, vf = /* @__PURE__ */ ue({
  __name: "ModalDialog",
  props: {
    id: { default: "ModalDialog" },
    modalClass: { default: os.modalClass },
    sizeClass: { default: os.sizeClass },
    closeButtonClass: { default: "bg-white dark:bg-black cursor-pointer rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black" },
    configureField: {}
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = Tl(), n = t, o = A(!1), r = A(""), u = {
      entering: { cls: "ease-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100", to: "opacity-0" }
    }, d = A(""), f = {
      entering: { cls: "ease-out duration-300", from: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95", to: "opacity-100 translate-y-0 sm:scale-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100 translate-y-0 sm:scale-100", to: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95" }
    };
    kt(o, () => {
      xt(u, r, o.value), xt(f, d, o.value), o.value || setTimeout(() => n("done"), 200);
    }), o.value = !0;
    const v = () => o.value = !1;
    ul("ModalProvider", {
      openModal: y
    });
    const $ = A(), b = A();
    function y(V, M) {
      $.value = V, b.value = M;
    }
    async function h(V) {
      b.value && b.value(V), $.value = void 0, b.value = void 0;
    }
    const k = (V) => {
      V.key === "Escape" && v();
    };
    return tt(() => window.addEventListener("keydown", k)), jt(() => window.removeEventListener("keydown", k)), (V, M) => {
      const Y = U("ModalLookup");
      return a(), i("div", {
        id: e.id,
        "data-transition-for": e.id,
        onMousedown: v,
        class: "relative z-10",
        "aria-labelledby": `${e.id}-title`,
        role: "dialog",
        "aria-modal": "true"
      }, [
        s("div", {
          class: g(["fixed inset-0 bg-gray-500/75 transition-opacity", r.value])
        }, null, 2),
        s("div", df, [
          s("div", cf, [
            s("div", {
              class: g([e.modalClass, e.sizeClass, d.value]),
              onMousedown: M[0] || (M[0] = He(() => {
              }, ["stop"]))
            }, [
              s("div", null, [
                G(l).closebutton ? O(V.$slots, "createform", { key: 0 }) : (a(), i("div", ff, [
                  s("button", {
                    type: "button",
                    onClick: v,
                    class: g(e.closeButtonClass),
                    title: "Close"
                  }, [...M[1] || (M[1] = [
                    s("span", { class: "sr-only" }, "Close", -1),
                    s("svg", {
                      class: "h-6 w-6",
                      xmlns: "http://www.w3.org/2000/svg",
                      fill: "none",
                      viewBox: "0 0 24 24",
                      stroke: "currentColor",
                      "aria-hidden": "true"
                    }, [
                      s("path", {
                        "stroke-linecap": "round",
                        "stroke-linejoin": "round",
                        "stroke-width": "2",
                        d: "M6 18L18 6M6 6l12 12"
                      })
                    ], -1)
                  ])], 2)
                ])),
                O(V.$slots, "default")
              ])
            ], 34),
            O(V.$slots, "bottom")
          ])
        ]),
        $.value?.name == "ModalLookup" && $.value.ref ? (a(), X(Y, {
          key: 0,
          "ref-info": $.value.ref,
          onDone: h,
          configureField: e.configureField
        }, null, 8, ["ref-info", "configureField"])) : x("", !0)
      ], 40, uf);
    };
  }
}), mf = {
  class: "pt-2 overflow-auto",
  style: { "min-height": "620px" }
}, gf = { class: "mt-3 pl-5 flex flex-wrap items-center" }, pf = { class: "hidden sm:block text-xl leading-6 font-medium text-gray-900 dark:text-gray-50 mr-3" }, yf = { class: "hidden md:inline" }, hf = { class: "flex pb-1 sm:pb-0" }, bf = ["title"], wf = ["disabled"], xf = ["disabled"], kf = ["disabled"], Cf = ["disabled"], $f = {
  key: 0,
  class: "flex pb-1 sm:pb-0"
}, Lf = { class: "px-4 text-lg text-black dark:text-white" }, Vf = { key: 0 }, Mf = { key: 1 }, Sf = { key: 2 }, Tf = {
  key: 1,
  class: "pl-2 mt-1"
}, Ff = { class: "whitespace-nowrap" }, Af = {
  key: 2,
  class: "pl-2"
}, If = { class: "flex pb-1 sm:pb-0" }, Pf = {
  key: 0,
  class: "pl-2"
}, Df = { class: "mr-1" }, Bf = {
  key: 0,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, jf = {
  key: 1,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Of = { key: 1 }, Hf = { key: 4 }, Rf = { key: 0 }, Ef = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, Nf = { class: "mr-1 select-none" }, zf = {
  key: 1,
  class: "flex justify-between items-center"
}, Uf = { class: "mr-1 select-none" }, Ys = 25, qf = /* @__PURE__ */ ue({
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
    const l = e, n = t, o = Tl(), { config: r } = Ot(), { metadataApi: u, filterDefinitions: d } = vt(), f = Ne("client"), v = r.value.storage, p = c(() => l.toolbarButtonClass ?? ge.toolbarButtonClass), $ = c(() => d.value), b = A({ take: Ys }), y = A(new Ge()), h = A(l.skip), k = A(!1), V = A(), M = (Z) => typeof Z == "string" ? Z.split(",") : Z || [];
    function Y(Z, H) {
      return ge.getTableRowClass("fullWidth", H, !1, !0);
    }
    function E() {
      let Z = M(l.selectedColumns);
      return Z.length > 0 ? Z : [];
    }
    const D = c(() => Je(l.refInfo.model)), Q = c(() => {
      let H = E().map((Ie) => Ie.toLowerCase());
      const Ae = et(D.value);
      return H.length > 0 ? H.map((Ie) => Ae.find((Ke) => Ke.name.toLowerCase() === Ie)).filter((Ie) => Ie != null) : Ae;
    }), q = c(() => {
      let Z = Q.value.map((Ae) => Ae.name), H = M(b.value.selectedColumns).map((Ae) => Ae.toLowerCase());
      return H.length > 0 ? Z.filter((Ae) => H.includes(Ae.toLowerCase())) : Z;
    }), S = c(() => b.value.take ?? Ys), L = c(() => (y.value.response ? ye(y.value.response, "results") : null) ?? []), le = c(() => y.value.response?.total ?? L.value.length ?? 0), ee = c(() => h.value > 0), j = c(() => h.value > 0), K = c(() => L.value.length >= S.value), _ = c(() => L.value.length >= S.value), te = A([]), se = c(() => te.value.some((Z) => Z.settings.filters.length > 0 || !!Z.settings.sort)), R = c(() => te.value.map((Z) => Z.settings.filters.length).reduce((Z, H) => Z + H, 0)), C = c(() => Jt(D.value)), P = c(() => u.value?.operations.find((Z) => Z.dataModel?.name == l.refInfo.model && Re.isAnyQuery(Z))), de = A(), w = A(!1), m = A(), W = c(() => Ht(l.refInfo.model)), ve = c(() => bt.forType(W.value, u.value)), J = c(() => W.value || P.value?.dataModel.name), z = c(() => l.modelTitle || J.value), B = c(() => l.newButtonLabel || `New ${z.value}`), ne = c(() => nl(ve.value.Create)), ie = A(), ae = A(!1);
    function oe() {
      ae.value = !0;
    }
    function me() {
      ae.value = !1;
    }
    async function ce(Z) {
      me(), n("done", Z);
    }
    function fe(Z) {
      ie.value && (Object.assign(ie.value?.model, Z), console.log("setCreate", JSON.stringify(Z, null, 2)), Fe());
    }
    function Fe() {
      ie.value?.forceUpdate(), Pe()?.proxy?.$forceUpdate();
    }
    const je = () => `${l.id}/ApiPrefs/${l.refInfo.model}`, xe = (Z) => `Column/${l.id}:${l.refInfo.model}.${Z}`;
    async function $e(Z) {
      h.value += Z, h.value < 0 && (h.value = 0);
      var H = Math.floor(le.value / S.value) * S.value;
      h.value > H && (h.value = H), await at();
    }
    async function Me(Z, H) {
      n("done", Z);
    }
    function qe() {
      n("done", null);
    }
    function nt(Z, H) {
      let Ae = H.target;
      if (Ae?.tagName !== "TD") {
        let Ie = Ae?.closest("TABLE")?.getBoundingClientRect(), Ke = te.value.find((it) => it.name.toLowerCase() == Z.toLowerCase());
        if (Ke && Ie) {
          let it = 318, Et = (H.target?.tagName === "DIV" ? H.target : H.target?.closest("DIV")).getBoundingClientRect(), Be = it + 25;
          m.value = {
            column: Ke,
            topLeft: {
              x: Math.max(Math.floor(Et.x + 25), Be),
              y: Math.floor(115)
            }
          };
        }
      }
    }
    function $t() {
      m.value = null;
    }
    async function Lt(Z) {
      let H = m.value?.column;
      H && (H.settings = Z, v.setItem(xe(H.name), JSON.stringify(H.settings)), await at()), m.value = null;
    }
    async function Qe(Z) {
      v.setItem(xe(Z.name), JSON.stringify(Z.settings)), await at();
    }
    async function lt(Z) {
      w.value = !1, b.value = Z, v.setItem(je(), JSON.stringify(Z)), await at();
    }
    async function at() {
      await ze(Xe());
    }
    async function ze(Z) {
      const H = P.value;
      if (!H) {
        console.error(`No Query API was found for ${l.refInfo.model}`);
        return;
      }
      let Ae = al(H, Z), Ie = on((Rt) => {
        y.value.response = y.value.error = void 0, k.value = Rt;
      }), Ke = await f.api(Ae);
      Ie(), wt(() => y.value = Ke);
      let it = ye(Ke.response, "results") || [];
      !Ke.succeeded || it.label == 0;
    }
    function Xe() {
      let Z = {
        include: "total",
        take: S.value
      }, H = M(b.value.selectedColumns || l.selectedColumns);
      if (H.length > 0) {
        let Ie = C.value;
        Ie && H.includes(Ie.name) && (H = [Ie.name, ...H]), Z.fields = H.join(",");
      }
      let Ae = [];
      return te.value.forEach((Ie) => {
        Ie.settings.sort && Ae.push((Ie.settings.sort === "DESC" ? "-" : "") + Ie.name), Ie.settings.filters.forEach((Ke) => {
          let it = Ke.key.replace("%", Ie.name);
          Z[it] = Ke.value;
        });
      }), typeof Z.skip > "u" && h.value > 0 && (Z.skip = h.value), Ae.length > 0 && (Z.orderBy = Ae.join(",")), Z;
    }
    async function ot() {
      te.value.forEach((Z) => {
        Z.settings = { filters: [] }, v.removeItem(xe(Z.name));
      }), await at();
    }
    return tt(async () => {
      const Z = l.prefs || Vl(v.getItem(je()));
      Z && (b.value = Z), te.value = Q.value.map((H) => ({
        name: H.name,
        type: H.type,
        meta: H,
        settings: Object.assign(
          {
            filters: []
          },
          Vl(v.getItem(xe(H.name)))
        )
      })), isNaN(l.skip) || (h.value = l.skip), await at();
    }), (Z, H) => {
      const Ae = U("AutoCreateForm"), Ie = U("ErrorSummary"), Ke = U("Loading"), it = U("SettingsIcons"), Rt = U("DataGrid"), Et = U("ModalDialog");
      return a(), i(Ve, null, [
        e.refInfo ? (a(), X(Et, {
          key: 0,
          ref_key: "modalDialog",
          ref: de,
          id: e.id,
          onDone: qe
        }, {
          default: we(() => [
            s("div", mf, [
              s("div", gf, [
                s("h3", pf, [
                  H[9] || (H[9] = he(" Select ", -1)),
                  s("span", yf, T(G(De)(e.refInfo.model)), 1)
                ]),
                s("div", hf, [
                  e.showPreferences ? (a(), i("button", {
                    key: 0,
                    type: "button",
                    class: "pl-2 text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                    title: `${e.refInfo.model} Preferences`,
                    onClick: H[0] || (H[0] = (Be) => w.value = !w.value)
                  }, [...H[10] || (H[10] = [
                    s("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("g", {
                        "stroke-width": "1.5",
                        fill: "none"
                      }, [
                        s("path", {
                          d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
                          stroke: "currentColor"
                        })
                      ])
                    ], -1)
                  ])], 8, bf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 1,
                    type: "button",
                    class: g(["pl-2", ee.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "First page",
                    disabled: !ee.value,
                    onClick: H[1] || (H[1] = (Be) => $e(-le.value))
                  }, [...H[11] || (H[11] = [
                    s("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, wf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 2,
                    type: "button",
                    class: g(["pl-2", j.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Previous page",
                    disabled: !j.value,
                    onClick: H[2] || (H[2] = (Be) => $e(-S.value))
                  }, [...H[12] || (H[12] = [
                    s("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, xf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 3,
                    type: "button",
                    class: g(["pl-2", K.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Next page",
                    disabled: !K.value,
                    onClick: H[3] || (H[3] = (Be) => $e(S.value))
                  }, [...H[13] || (H[13] = [
                    s("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, kf)) : x("", !0),
                  e.showPagingNav ? (a(), i("button", {
                    key: 4,
                    type: "button",
                    class: g(["pl-2", _.value ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Last page",
                    disabled: !_.value,
                    onClick: H[4] || (H[4] = (Be) => $e(le.value))
                  }, [...H[14] || (H[14] = [
                    s("svg", {
                      class: "w-8 h-8",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
                        fill: "currentColor"
                      })
                    ], -1)
                  ])], 10, Cf)) : x("", !0)
                ]),
                e.showPagingInfo ? (a(), i("div", $f, [
                  s("div", Lf, [
                    k.value ? (a(), i("span", Vf, "Querying...")) : x("", !0),
                    L.value.length ? (a(), i("span", Mf, [
                      H[15] || (H[15] = s("span", { class: "hidden xl:inline" }, " Showing Results ", -1)),
                      he(" " + T(h.value + 1) + " - " + T(Math.min(h.value + L.value.length, le.value)) + " ", 1),
                      s("span", null, " of " + T(le.value), 1)
                    ])) : y.value.completed ? (a(), i("span", Sf, "No Results")) : x("", !0)
                  ])
                ])) : x("", !0),
                ve.value.Create && ne.value ? (a(), i("div", Tf, [
                  s("button", {
                    type: "button",
                    onClick: H[5] || (H[5] = (Be) => oe()),
                    title: "modelTitle",
                    class: g(G(ge).toolbarButtonClass)
                  }, [
                    H[16] || (H[16] = s("svg", {
                      class: "w-5 h-5 mr-1 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50",
                      xmlns: "http://www.w3.org/2000/svg",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        d: "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z",
                        fill: "currentColor"
                      })
                    ], -1)),
                    s("span", Ff, T(B.value), 1)
                  ], 2),
                  ae.value ? (a(), X(Ae, {
                    key: 0,
                    ref_key: "createForm",
                    ref: ie,
                    type: ve.value.Create.request.name,
                    configure: e.configureField,
                    onDone: me,
                    onSave: ce
                  }, {
                    header: we(() => [
                      O(Z.$slots, "formheader", {
                        form: "create",
                        formInstance: ie.value,
                        apis: ve.value,
                        type: J.value,
                        updateModel: fe
                      })
                    ]),
                    footer: we(() => [
                      O(Z.$slots, "formfooter", {
                        form: "create",
                        formInstance: ie.value,
                        apis: ve.value,
                        type: J.value,
                        updateModel: fe
                      })
                    ]),
                    _: 3
                  }, 8, ["type", "configure"])) : x("", !0)
                ])) : x("", !0),
                se.value && e.showResetPreferences ? (a(), i("div", Af, [
                  s("button", {
                    type: "button",
                    onClick: ot,
                    title: "Reset Preferences & Filters",
                    class: g(p.value)
                  }, [...H[17] || (H[17] = [
                    s("svg", {
                      class: "w-5 h-5",
                      xmlns: "http://www.w3.org/2000/svg",
                      "aria-hidden": "true",
                      viewBox: "0 0 24 24"
                    }, [
                      s("path", {
                        fill: "currentColor",
                        d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
                      })
                    ], -1)
                  ])], 2)
                ])) : x("", !0),
                s("div", If, [
                  e.showFiltersView && R.value > 0 ? (a(), i("div", Pf, [
                    s("button", {
                      type: "button",
                      onClick: H[6] || (H[6] = (Be) => V.value = V.value == "filters" ? null : "filters"),
                      class: g(p.value),
                      "aria-expanded": "false"
                    }, [
                      H[20] || (H[20] = s("svg", {
                        class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
                        "aria-hidden": "true",
                        xmlns: "http://www.w3.org/2000/svg",
                        viewBox: "0 0 20 20",
                        fill: "currentColor"
                      }, [
                        s("path", {
                          "fill-rule": "evenodd",
                          d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
                          "clip-rule": "evenodd"
                        })
                      ], -1)),
                      s("span", Df, T(R.value) + " " + T(R.value == 1 ? "Filter" : "Filters"), 1),
                      V.value != "filters" ? (a(), i("svg", Bf, [...H[18] || (H[18] = [
                        s("path", {
                          "fill-rule": "evenodd",
                          d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
                          "clip-rule": "evenodd"
                        }, null, -1)
                      ])])) : (a(), i("svg", jf, [...H[19] || (H[19] = [
                        s("path", {
                          "fill-rule": "evenodd",
                          d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
                          "clip-rule": "evenodd"
                        }, null, -1)
                      ])]))
                    ], 2)
                  ])) : x("", !0)
                ])
              ]),
              V.value == "filters" ? (a(), X(Ds, {
                key: 0,
                class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
                definitions: $.value,
                columns: te.value,
                onDone: H[7] || (H[7] = (Be) => V.value = null),
                onChange: Qe
              }, null, 8, ["definitions", "columns"])) : x("", !0),
              m.value ? (a(), i("div", Of, [
                pe(Ps, {
                  definitions: $.value,
                  column: m.value.column,
                  "top-left": m.value.topLeft,
                  onDone: $t,
                  onSave: Lt
                }, null, 8, ["definitions", "column", "top-left"])
              ])) : x("", !0),
              y.value.error ? (a(), X(Ie, {
                key: 2,
                status: y.value.error
              }, null, 8, ["status"])) : k.value ? (a(), X(Ke, { key: 3 })) : (a(), i("div", Hf, [
                L.value.length ? (a(), i("div", Rf, [
                  pe(Rt, {
                    id: e.id,
                    items: L.value,
                    type: e.refInfo.model,
                    "selected-columns": q.value,
                    onFiltersChanged: at,
                    tableStyle: "fullWidth",
                    rowClass: Y,
                    onRowSelected: Me,
                    onHeaderSelected: nt
                  }, us({
                    header: we(({ column: Be, label: yt }) => [
                      e.allowFiltering && (!l.canFilter || l.canFilter(Be)) ? (a(), i("div", Ef, [
                        s("span", Nf, T(yt), 1),
                        pe(it, {
                          column: te.value.find((st) => st.name.toLowerCase() === Be.toLowerCase()),
                          "is-open": m.value?.column.name === Be
                        }, null, 8, ["column", "is-open"])
                      ])) : (a(), i("div", zf, [
                        s("span", Uf, T(yt), 1)
                      ]))
                    ]),
                    _: 2
                  }, [
                    Se(Object.keys(G(o)), (Be) => ({
                      name: Be,
                      fn: we((yt) => [
                        O(Z.$slots, Be, $l(ln(yt)))
                      ])
                    }))
                  ]), 1032, ["id", "items", "type", "selected-columns"])
                ])) : x("", !0)
              ]))
            ])
          ]),
          _: 3
        }, 8, ["id"])) : x("", !0),
        w.value ? (a(), X(Bs, {
          key: 1,
          columns: Q.value,
          prefs: b.value,
          onDone: H[8] || (H[8] = (Be) => w.value = !1),
          onSave: lt
        }, null, 8, ["columns", "prefs"])) : x("", !0)
      ], 64);
    };
  }
}), Qf = { class: "sm:hidden" }, Kf = ["for"], _f = ["id", "name"], Zf = ["value"], Wf = { class: "hidden sm:block" }, Gf = { class: "border-b border-gray-200" }, Jf = {
  class: "-mb-px flex",
  "aria-label": "Tabs"
}, Xf = ["onClick"], Yf = /* @__PURE__ */ ue({
  __name: "Tabs",
  props: {
    tabs: {},
    id: { default: "tabs" },
    param: { default: "tab" },
    label: { type: Function, default: (e) => De(e) },
    selected: {},
    tabClass: {},
    bodyClass: { default: "p-4" },
    url: { type: Boolean, default: !0 },
    clearQuery: { type: Boolean, default: !1 }
  },
  setup(e) {
    const t = e, l = c(() => Object.keys(t.tabs)), n = (p) => t.label ? t.label(p) : De(p), o = c(() => t.id || "tabs"), r = c(() => t.param || "tab"), u = A();
    function d(p) {
      if (u.value = p, t.url) {
        const $ = l.value[0];
        Ms({ tab: p === $ ? void 0 : p }, t.clearQuery);
      }
    }
    function f(p) {
      return u.value === p;
    }
    const v = c(() => `${100 / Object.keys(t.tabs).length}%`);
    return tt(() => {
      if (u.value = t.selected || Object.keys(t.tabs)[0], t.url) {
        const p = location.search ? location.search : location.hash.includes("?") ? "?" + bl(location.hash, "?") : "", b = Gl(p)[r.value];
        b && (u.value = b);
      }
    }), (p, $) => (a(), i("div", null, [
      s("div", Qf, [
        s("label", {
          for: o.value,
          class: "sr-only"
        }, "Select a tab", 8, Kf),
        s("select", {
          id: o.value,
          name: o.value,
          class: "block w-full rounded-md border-gray-300 focus:border-indigo-500 focus:ring-indigo-500",
          onChange: $[0] || ($[0] = (b) => d(b.target?.value))
        }, [
          (a(!0), i(Ve, null, Se(l.value, (b) => (a(), i("option", {
            key: b,
            value: b
          }, T(n(b)), 9, Zf))), 128))
        ], 40, _f)
      ]),
      s("div", Wf, [
        s("div", Gf, [
          s("nav", Jf, [
            (a(!0), i(Ve, null, Se(l.value, (b) => (a(), i("a", {
              href: "#",
              onClick: He((y) => d(b), ["prevent"]),
              style: is({ width: v.value }),
              class: g([f(b) ? "border-indigo-500 text-indigo-600 py-4 px-1 text-center border-b-2 font-medium text-sm" : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 py-4 px-1 text-center border-b-2 font-medium text-sm", e.tabClass])
            }, T(n(b)), 15, Xf))), 256))
          ])
        ])
      ]),
      s("div", {
        class: g(e.bodyClass)
      }, [
        (a(), X(sn(e.tabs[u.value])))
      ], 2)
    ]));
  }
}), ev = /* @__PURE__ */ ue({
  __name: "DarkModeToggle",
  setup(e) {
    const t = typeof document < "u" ? document.documentElement : null, l = () => !!t?.classList.contains("dark"), n = A(localStorage.getItem("color-scheme") == "dark");
    function o() {
      l() ? t?.classList.remove("dark") : t?.classList.add("dark"), n.value = l(), localStorage.setItem("color-scheme", n.value ? "dark" : "light");
    }
    return (r, u) => (a(), i("button", {
      type: "button",
      class: "bg-gray-200 dark:bg-gray-700 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black",
      role: "switch",
      "aria-checked": "false",
      onClick: u[0] || (u[0] = (d) => o())
    }, [
      s("span", {
        class: g(`${n.value ? "translate-x-0" : "translate-x-5"} pointer-events-none relative inline-block h-5 w-5 rounded-full bg-white dark:bg-black shadow transform ring-0 transition ease-in-out duration-200`)
      }, [
        s("span", {
          class: g(`${n.value ? "opacity-100 ease-in duration-200" : "opacity-0 ease-out duration-100"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, [...u[1] || (u[1] = [
          s("svg", {
            xmlns: "http://www.w3.org/2000/svg",
            class: "h-4 w-4 text-gray-400",
            preserveAspectRatio: "xMidYMid meet",
            viewBox: "0 0 32 32"
          }, [
            s("path", {
              fill: "currentColor",
              d: "M13.502 5.414a15.075 15.075 0 0 0 11.594 18.194a11.113 11.113 0 0 1-7.975 3.39c-.138 0-.278.005-.418 0a11.094 11.094 0 0 1-3.2-21.584M14.98 3a1.002 1.002 0 0 0-.175.016a13.096 13.096 0 0 0 1.825 25.981c.164.006.328 0 .49 0a13.072 13.072 0 0 0 10.703-5.555a1.01 1.01 0 0 0-.783-1.565A13.08 13.08 0 0 1 15.89 4.38A1.015 1.015 0 0 0 14.98 3Z"
            })
          ], -1)
        ])], 2),
        s("span", {
          class: g(`${n.value ? "opacity-0 ease-out duration-100" : "opacity-100 ease-in duration-200"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, [...u[2] || (u[2] = [
          s("svg", {
            xmlns: "http://www.w3.org/2000/svg",
            class: "h-4 w-4 text-indigo-600",
            preserveAspectRatio: "xMidYMid meet",
            viewBox: "0 0 32 32"
          }, [
            s("path", {
              fill: "currentColor",
              d: "M16 12.005a4 4 0 1 1-4 4a4.005 4.005 0 0 1 4-4m0-2a6 6 0 1 0 6 6a6 6 0 0 0-6-6ZM5.394 6.813L6.81 5.399l3.505 3.506L8.9 10.319zM2 15.005h5v2H2zm3.394 10.193L8.9 21.692l1.414 1.414l-3.505 3.506zM15 25.005h2v5h-2zm6.687-1.9l1.414-1.414l3.506 3.506l-1.414 1.414zm3.313-8.1h5v2h-5zm-3.313-6.101l3.506-3.506l1.414 1.414l-3.506 3.506zM15 2.005h2v5h-2z"
            })
          ], -1)
        ])], 2)
      ], 2)
    ]));
  }
}), tv = { key: 0 }, lv = {
  key: 1,
  class: "min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8"
}, sv = { class: "sm:mx-auto sm:w-full sm:max-w-md" }, nv = { class: "mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-gray-50" }, av = {
  key: 0,
  class: "mt-4 text-center text-sm text-gray-600 dark:text-gray-300"
}, ov = { class: "relative z-0 inline-flex shadow-sm rounded-md" }, iv = ["onClick"], rv = { class: "mt-8 sm:mx-auto sm:w-full sm:max-w-md" }, uv = { class: "bg-white dark:bg-black py-8 px-4 shadow sm:rounded-lg sm:px-10" }, dv = { class: "mt-8" }, cv = {
  key: 1,
  class: "mt-6"
}, fv = { class: "mt-6 grid grid-cols-3 gap-3" }, vv = ["href", "title"], mv = {
  key: 1,
  class: "h-5 w-5 text-gray-700 dark:text-gray-200",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 32 32"
}, gv = /* @__PURE__ */ ue({
  __name: "SignIn",
  props: {
    provider: {},
    title: { default: "Sign In" },
    tabs: { type: [Boolean, String], default: !0 },
    oauth: { type: [Boolean, String], default: !0 }
  },
  emits: ["login"],
  setup(e, { emit: t }) {
    const l = e, n = t, { getMetadata: o, createDto: r } = vt(), u = fl(), d = Ne("client"), { signIn: f } = Is(), v = o({ assert: !0 }), p = v.plugins.auth, $ = document.baseURI, b = v.app.baseUrl, y = A(r("Authenticate")), h = A(new Ge()), k = A(l.provider);
    tt(() => {
      p?.authProviders.map((j) => j.formLayout).filter((j) => j).forEach((j) => j.forEach(
        (K) => y.value[K.id] = K.type === "checkbox" ? !1 : ""
      ));
    });
    const V = c(() => p?.authProviders.filter((j) => j.formLayout) || []), M = c(() => V.value[0] || {}), Y = c(() => V.value[Math.max(V.value.length - 1, 0)] || {}), E = c(() => (k.value ? p?.authProviders.find((j) => j.name === k.value) : null) ?? M.value), D = (j) => j === !1 || j === "false";
    function Q(j) {
      return j.label || j.navItem && j.navItem.label;
    }
    const q = c(() => (E.value?.formLayout || []).map((j) => Object.assign({}, j, {
      type: j.type?.toLowerCase(),
      autocomplete: j.autocomplete || (j.type?.toLowerCase() === "password" ? "current-password" : void 0) || (j.id.toLowerCase() === "username" ? "username" : void 0),
      css: Object.assign({ field: "col-span-12" }, j.css)
    }))), S = c(() => D(l.oauth) ? [] : p?.authProviders.filter((j) => j.type === "oauth") || []), L = c(() => {
      let j = Pa(
        p?.authProviders.filter((_) => _.formLayout && _.formLayout.length > 0),
        (_, te) => {
          let se = Q(te) || ut(te.name);
          _[se] = te.name === M.value.name ? "" : te.name;
        }
      );
      const K = E.value;
      return K && D(l.tabs) && (j = { [Q(K) || ut(K.name)]: K }), j;
    }), le = c(() => {
      let j = q.value.map((K) => K.id).filter((K) => K);
      return h.value.summaryMessage(j);
    });
    async function ee() {
      if (y.value.provider = E.value.name, E.value.name === "authsecret" ? (d.headers.set("authsecret", y.value.authsecret), y.value = r("Authenticate")) : E.value.name === "basic" ? (d.setCredentials(y.value.UserName, y.value.Password), y.value = r("Authenticate"), y.value.UserName = null, y.value.Password = null) : (E.value.type === "Bearer" || E.value.name === "jwt") && (d.bearerToken = y.value.BearerToken, y.value = r("Authenticate")), h.value = await u.api(y.value), h.value.succeeded) {
        const j = h.value.response;
        f(j), n("login", j), h.value = new Ge(), y.value = r("Authenticate");
      }
    }
    return (j, K) => {
      const _ = U("ErrorSummary"), te = U("AutoFormFields"), se = U("PrimaryButton"), R = U("Icon"), C = pa("href");
      return G(p) ? (a(), i("div", lv, [
        s("div", sv, [
          s("h2", nv, T(e.title), 1),
          Object.keys(L.value).length > 1 ? (a(), i("p", av, [
            s("span", ov, [
              (a(!0), i(Ve, null, Se(L.value, (P, de) => Pt((a(), i("a", {
                onClick: (w) => k.value = P,
                class: g([
                  P === "" || P === Y.value.name ? "rounded-l-md" : P === Y.value.name ? "rounded-r-md -ml-px" : "-ml-px",
                  k.value === P ? "z-10 outline-none ring-1 ring-indigo-500 border-indigo-500" : "",
                  "cursor-pointer relative inline-flex items-center px-4 py-1 border border-gray-300 dark:border-gray-600 bg-white dark:bg-black text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-900"
                ])
              }, [
                he(T(de), 1)
              ], 10, iv)), [
                [C, { provider: P }]
              ])), 256))
            ])
          ])) : x("", !0)
        ]),
        s("div", rv, [
          le.value ? (a(), X(_, {
            key: 0,
            class: "mb-3",
            errorSummary: le.value
          }, null, 8, ["errorSummary"])) : x("", !0),
          s("div", uv, [
            q.value.length ? (a(), i("form", {
              key: 0,
              onSubmit: He(ee, ["prevent"])
            }, [
              pe(te, {
                modelValue: y.value,
                formLayout: q.value,
                api: h.value,
                hideSummary: !0,
                "divide-class": "",
                "space-class": "space-y-6"
              }, null, 8, ["modelValue", "formLayout", "api"]),
              s("div", dv, [
                pe(se, { class: "w-full" }, {
                  default: we(() => [...K[0] || (K[0] = [
                    he("Sign In", -1)
                  ])]),
                  _: 1
                })
              ])
            ], 32)) : x("", !0),
            S.value.length ? (a(), i("div", cv, [
              K[2] || (K[2] = Sl('<div class="relative"><div class="absolute inset-0 flex items-center"><div class="w-full border-t border-gray-300 dark:border-gray-600"></div></div><div class="relative flex justify-center text-sm"><span class="px-2 bg-white text-gray-500 dark:text-gray-400"> Or continue with </span></div></div>', 1)),
              s("div", fv, [
                (a(!0), i(Ve, null, Se(S.value, (P) => (a(), i("div", null, [
                  s("a", {
                    href: G(b) + P.navItem.href + "?continue=" + G($),
                    title: Q(P),
                    class: "w-full inline-flex justify-center py-2 px-4 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-black text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-900"
                  }, [
                    P.icon ? (a(), X(R, {
                      key: 0,
                      image: P.icon,
                      class: "h-5 w-5 text-gray-700 dark:text-gray-200"
                    }, null, 8, ["image"])) : (a(), i("svg", mv, [...K[1] || (K[1] = [
                      s("path", {
                        d: "M16 8a5 5 0 1 0 5 5a5 5 0 0 0-5-5z",
                        fill: "currentColor"
                      }, null, -1),
                      s("path", {
                        d: "M16 2a14 14 0 1 0 14 14A14.016 14.016 0 0 0 16 2zm7.992 22.926A5.002 5.002 0 0 0 19 20h-6a5.002 5.002 0 0 0-4.992 4.926a12 12 0 1 1 15.985 0z",
                        fill: "currentColor"
                      }, null, -1)
                    ])]))
                  ], 8, vv)
                ]))), 256))
              ])
            ])) : x("", !0)
          ])
        ])
      ])) : (a(), i("div", tv, "No Auth Plugin"));
    };
  }
}), pv = ["for"], yv = {
  key: 1,
  class: "border border-gray-200 flex justify-between shadow-sm"
}, hv = { class: "p-2 flex flex-wrap gap-x-4" }, bv = {
  key: 0,
  class: "p-2 flex flex-wrap gap-x-4"
}, wv = ["href"], xv = { class: "" }, kv = ["name", "id", "label", "value", "rows", "disabled"], Cv = ["id"], $v = ["id"], Ye = "w-5 h-5 cursor-pointer select-none text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400", Lv = /* @__PURE__ */ ue({
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
    const n = e, o = l;
    let r = [], u = [], d = Ne("ApiState", void 0);
    const f = c(() => pt.call({ responseStatus: n.status ?? d?.error.value }, n.id)), v = c(() => n.label ?? De(ut(n.id))), p = "bold,italics,link,image,blockquote,code,heading,orderedList,unorderedList,strikethrough,undo,redo,help".split(","), $ = c(() => n.hide ? At(p, n.hide) : At(p, []));
    function b(w) {
      return $.value[w];
    }
    const y = c(() => Ct([
      "shadow-sm font-mono" + rt.base.replace("rounded-md", ""),
      f.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + rt.valid,
      n.inputClass
    ], "MarkdownInput", n.filterClass)), h = A();
    t({ props: n, textarea: h, updateModelValue: k, selection: M, hasSelection: V, selectionInfo: Y, insert: D, replace: E });
    function k(w) {
      o("update:modelValue", w);
    }
    function V() {
      return h.value.selectionStart !== h.value.selectionEnd;
    }
    function M() {
      const w = h.value;
      return w.value.substring(w.selectionStart, w.selectionEnd) || "";
    }
    function Y() {
      const w = h.value, m = w.value, W = w.selectionStart, ve = m.substring(W, w.selectionEnd) || "", J = m.substring(0, W), z = J.lastIndexOf(`
`);
      return {
        value: m,
        sel: ve,
        selPos: W,
        beforeSel: J,
        afterSel: m.substring(W),
        prevCRPos: z,
        beforeCR: z >= 0 ? J.substring(0, z + 1) : "",
        afterCR: z >= 0 ? J.substring(z + 1) : ""
      };
    }
    function E({ value: w, selectionStart: m, selectionEnd: W }) {
      W == null && (W = m), k(w), wt(() => {
        h.value.focus(), h.value.setSelectionRange(m, W);
      });
    }
    function D(w, m, W = "", { selectionAtEnd: ve, offsetStart: J, offsetEnd: z, filterValue: B, filterSelection: ne } = {}) {
      const ie = h.value;
      let ae = ie.value, oe = ie.selectionEnd;
      r.push({ value: ae, selectionStart: ie.selectionStart, selectionEnd: ie.selectionEnd }), u = [];
      const me = ie.selectionStart, ce = ie.selectionEnd;
      let fe = ae.substring(0, me), Fe = ae.substring(ce);
      const je = w && fe.endsWith(w) && Fe.startsWith(m);
      if (me == ce) {
        if (je ? (ae = fe.substring(0, fe.length - w.length) + Fe.substring(m.length), oe += -m.length) : (ae = fe + w + W + m + Fe, oe += w.length, J = 0, z = W?.length || 0, ve && (oe += z, z = 0)), B) {
          var $e = { pos: oe };
          ae = B(ae, $e), oe = $e.pos;
        }
      } else {
        var Me = ae.substring(me, ce);
        ne && (Me = ne(Me)), je ? (ae = fe.substring(0, fe.length - w.length) + Me + Fe.substring(m.length), J = -Me.length - w.length, z = Me.length) : (ae = fe + w + Me + m + Fe, J ? oe += (w + m).length : (oe = me, J = w.length, z = Me.length));
      }
      k(ae), wt(() => {
        ie.focus(), J = oe + (J || 0), z = (J || 0) + (z || 0), ie.setSelectionRange(J, z);
      });
    }
    const Q = () => D("**", "**", "bold"), q = () => D("_", "_", "italics"), S = () => D("~~", "~~", "strikethrough"), L = () => D("[", "](https://)", "", { offsetStart: -9, offsetEnd: 8 }), le = () => D(`
> `, `
`, "Blockquote", {}), ee = () => D("![](", ")");
    function j(w) {
      const m = M();
      if (m && !w.shiftKey)
        D("`", "`", "code");
      else {
        const W = n.lang || "js";
        m.indexOf(`
`) === -1 ? D("\n```" + W + `
`, "\n```\n", "// code") : D("```" + W + `
`, "```\n", "");
      }
    }
    function K() {
      if (V()) {
        let { sel: w, selPos: m, beforeSel: W, afterSel: ve, prevCRPos: J, beforeCR: z, afterCR: B } = Y();
        if (w.indexOf(`
`) === -1)
          D(`
 1. `, `
`);
        else if (!w.startsWith(" 1. ")) {
          let ae = 1;
          D("", "", " - ", {
            selectionAtEnd: !0,
            filterSelection: (oe) => " 1. " + oe.replace(/\n$/, "").replace(/\n/g, (me) => `
 ${++ae}. `) + `
`
          });
        } else
          D("", "", "", {
            filterValue: (ae, oe) => {
              if (J >= 0) {
                let me = B.replace(/^ - /, "");
                W = z + me, oe.pos -= B.length - me.length;
              }
              return W + ve;
            },
            filterSelection: (ae) => ae.replace(/^ 1. /g, "").replace(/\n \d+. /g, `
`)
          });
      } else
        D(`
 1. `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function _() {
      if (V()) {
        let { sel: w, selPos: m, beforeSel: W, afterSel: ve, prevCRPos: J, beforeCR: z, afterCR: B } = Y();
        w.indexOf(`
`) === -1 ? D(`
 - `, `
`) : !w.startsWith(" - ") ? D("", "", " - ", {
          selectionAtEnd: !0,
          filterSelection: (ae) => " - " + ae.replace(/\n$/, "").replace(/\n/g, `
 - `) + `
`
        }) : D("", "", "", {
          filterValue: (ae, oe) => {
            if (J >= 0) {
              let me = B.replace(/^ - /, "");
              W = z + me, oe.pos -= B.length - me.length;
            }
            return W + ve;
          },
          filterSelection: (ae) => ae.replace(/^ - /g, "").replace(/\n - /g, `
`)
        });
      } else
        D(`
 - `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function te() {
      const w = M(), m = w.indexOf(`
`) === -1;
      w ? m ? D(`
## `, `
`, "") : D("## ", "", "") : D(`
## `, `
`, "Heading", { offsetStart: -8, offsetEnd: 7 });
    }
    function se() {
      let { sel: w, selPos: m, beforeSel: W, afterSel: ve, prevCRPos: J, beforeCR: z, afterCR: B } = Y();
      !w.startsWith("//") && !B.startsWith("//") ? w ? D("", "", "//", {
        selectionAtEnd: !0,
        filterSelection: (ie) => "//" + ie.replace(/\n$/, "").replace(/\n/g, `
//`) + `
`
      }) : E({
        value: z + "//" + B + ve,
        selectionStart: m + 2
      }) : D("", "", "", {
        filterValue: (ie, ae) => {
          if (J >= 0) {
            let oe = B.replace(/^\/\//, "");
            W = z + oe, ae.pos -= B.length - oe.length;
          }
          return W + ve;
        },
        filterSelection: (ie) => ie.replace(/^\/\//g, "").replace(/\n\/\//g, `
`)
      });
    }
    const R = () => D(`/*
`, `*/
`, "");
    function C() {
      if (r.length === 0) return !1;
      const w = h.value, m = r.pop();
      return u.push({ value: w.value, selectionStart: w.selectionStart, selectionEnd: w.selectionEnd }), E(m), !0;
    }
    function P() {
      if (u.length === 0) return !1;
      const w = h.value, m = u.pop();
      return r.push({ value: w.value, selectionStart: w.selectionStart, selectionEnd: w.selectionEnd }), E(m), !0;
    }
    const de = () => null;
    return tt(() => {
      r = [], u = [];
      const w = h.value;
      w.onkeydown = (m) => {
        if (m.key === "Escape" || m.keyCode === 27) {
          o("close");
          return;
        }
        const W = String.fromCharCode(m.keyCode).toLowerCase();
        W === "	" ? (!m.shiftKey ? D("", "", "    ", {
          selectionAtEnd: !0,
          filterSelection: (J) => "    " + J.replace(/\n$/, "").replace(/\n/g, `
    `) + `
`
        }) : D("", "", "", {
          filterValue: (J, z) => {
            let { selPos: B, beforeSel: ne, afterSel: ie, prevCRPos: ae, beforeCR: oe, afterCR: me } = Y();
            if (ae >= 0) {
              let ce = me.replace(/\t/g, "    ").replace(/^ ? ? ? ?/, "");
              ne = oe + ce, z.pos -= me.length - ce.length;
            }
            return ne + ie;
          },
          filterSelection: (J) => J.replace(/\t/g, "    ").replace(/^ ? ? ? ?/g, "").replace(/\n    /g, `
`)
        }), m.preventDefault()) : m.ctrlKey ? W === "z" ? m.shiftKey ? P() && m.preventDefault() : C() && m.preventDefault() : W === "b" && !m.shiftKey ? (Q(), m.preventDefault()) : W === "h" && !m.shiftKey ? (te(), m.preventDefault()) : W === "i" && !m.shiftKey ? (q(), m.preventDefault()) : W === "q" && !m.shiftKey ? (le(), m.preventDefault()) : W === "k" ? m.shiftKey ? (ee(), m.preventDefault()) : (L(), m.preventDefault()) : W === "," || m.key === "<" || m.key === ">" || m.keyCode === 188 ? (j(m), m.preventDefault()) : W === "/" || m.key === "/" ? (se(), m.preventDefault()) : (W === "?" || m.key === "?") && m.shiftKey && (R(), m.preventDefault()) : m.altKey && (m.key === "1" || m.key === "0" ? (K(), m.preventDefault()) : m.key === "-" ? (_(), m.preventDefault()) : m.key === "s" && (S(), m.preventDefault()));
      };
    }), (w, m) => (a(), i("div", null, [
      O(w.$slots, "header", Ce({
        inputElement: h.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, w.$attrs)),
      v.value ? (a(), i("label", {
        key: 0,
        for: e.id,
        class: g(`mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, T(v.value), 11, pv)) : x("", !0),
      e.disabled ? x("", !0) : (a(), i("div", yv, [
        s("div", hv, [
          b("bold") ? (a(), i("svg", {
            key: 0,
            class: g(Ye),
            onClick: Q,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[1] || (m[1] = [
            s("title", null, "Bold text (CTRL+B)", -1),
            s("path", {
              fill: "currentColor",
              d: "M15.6 10.79c.97-.67 1.65-1.77 1.65-2.79c0-2.26-1.75-4-4-4H7v14h7.04c2.09 0 3.71-1.7 3.71-3.79c0-1.52-.86-2.82-2.15-3.42zM10 6.5h3c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5h-3v-3zm3.5 9H10v-3h3.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5z"
            }, null, -1)
          ])])) : x("", !0),
          b("italics") ? (a(), i("svg", {
            key: 1,
            class: g(Ye),
            onClick: q,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[2] || (m[2] = [
            s("title", null, "Italics (CTRL+I)", -1),
            s("path", {
              fill: "currentColor",
              d: "M10 4v3h2.21l-3.42 8H6v3h8v-3h-2.21l3.42-8H18V4h-8z"
            }, null, -1)
          ])])) : x("", !0),
          b("link") ? (a(), i("svg", {
            key: 2,
            class: g(Ye),
            onClick: L,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[3] || (m[3] = [
            s("title", null, "Insert Link (CTRL+K)", -1),
            s("path", {
              fill: "currentColor",
              d: "M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7a5 5 0 0 0-5 5a5 5 0 0 0 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1M8 13h8v-2H8v2m9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1c0 1.71-1.39 3.1-3.1 3.1h-4V17h4a5 5 0 0 0 5-5a5 5 0 0 0-5-5Z"
            }, null, -1)
          ])])) : x("", !0),
          b("blockquote") ? (a(), i("svg", {
            key: 3,
            class: g(Ye),
            onClick: le,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[4] || (m[4] = [
            s("title", null, "Blockquote (CTRL+Q)", -1),
            s("path", {
              fill: "currentColor",
              d: "m15 17l2-4h-4V6h7v7l-2 4h-3Zm-9 0l2-4H4V6h7v7l-2 4H6Z"
            }, null, -1)
          ])])) : x("", !0),
          b("image") ? (a(), i("svg", {
            key: 4,
            class: g(Ye),
            onClick: ee,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[5] || (m[5] = [
            s("title", null, "Insert Image (CTRL+SHIFT+L)", -1),
            s("path", {
              fill: "currentColor",
              d: "M2.992 21A.993.993 0 0 1 2 20.007V3.993A1 1 0 0 1 2.992 3h18.016c.548 0 .992.445.992.993v16.014a1 1 0 0 1-.992.993H2.992ZM20 15V5H4v14L14 9l6 6Zm0 2.828l-6-6L6.828 19H20v-1.172ZM8 11a2 2 0 1 1 0-4a2 2 0 0 1 0 4Z"
            }, null, -1)
          ])])) : x("", !0),
          b("code") ? (a(), i("svg", {
            key: 5,
            class: g(Ye),
            onClick: j,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[6] || (m[6] = [
            s("title", null, "Insert Code (CTRL+<)", -1),
            s("path", {
              fill: "currentColor",
              d: "m8 18l-6-6l6-6l1.425 1.425l-4.6 4.6L9.4 16.6L8 18Zm8 0l-1.425-1.425l4.6-4.6L14.6 7.4L16 6l6 6l-6 6Z"
            }, null, -1)
          ])])) : x("", !0),
          b("heading") ? (a(), i("svg", {
            key: 6,
            class: g(Ye),
            onClick: te,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[7] || (m[7] = [
            s("title", null, "H2 Heading (CTRL+H)", -1),
            s("path", {
              fill: "currentColor",
              d: "M7 20V7H2V4h13v3h-5v13H7Zm9 0v-8h-3V9h9v3h-3v8h-3Z"
            }, null, -1)
          ])])) : x("", !0),
          b("orderedList") ? (a(), i("svg", {
            key: 7,
            class: g(Ye),
            icon: "",
            onClick: K,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[8] || (m[8] = [
            s("title", null, "Numbered List (ALT+1)", -1),
            s("path", {
              fill: "currentColor",
              d: "M3 22v-1.5h2.5v-.75H4v-1.5h1.5v-.75H3V16h3q.425 0 .713.288T7 17v1q0 .425-.288.713T6 19q.425 0 .713.288T7 20v1q0 .425-.288.713T6 22H3Zm0-7v-2.75q0-.425.288-.713T4 11.25h1.5v-.75H3V9h3q.425 0 .713.288T7 10v1.75q0 .425-.288.713T6 12.75H4.5v.75H7V15H3Zm1.5-7V3.5H3V2h3v6H4.5ZM9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9Z"
            }, null, -1)
          ])])) : x("", !0),
          b("unorderedList") ? (a(), i("svg", {
            key: 8,
            class: g(Ye),
            onClick: _,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[9] || (m[9] = [
            s("title", null, "Bulleted List (ALT+-)", -1),
            s("path", {
              fill: "currentColor",
              d: "M9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9ZM5 20q-.825 0-1.413-.588T3 18q0-.825.588-1.413T5 16q.825 0 1.413.588T7 18q0 .825-.588 1.413T5 20Zm0-6q-.825 0-1.413-.588T3 12q0-.825.588-1.413T5 10q.825 0 1.413.588T7 12q0 .825-.588 1.413T5 14Zm0-6q-.825 0-1.413-.588T3 6q0-.825.588-1.413T5 4q.825 0 1.413.588T7 6q0 .825-.588 1.413T5 8Z"
            }, null, -1)
          ])])) : x("", !0),
          b("strikethrough") ? (a(), i("svg", {
            key: 9,
            class: g(Ye),
            onClick: S,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[10] || (m[10] = [
            s("title", null, "Strike Through (ALT+S)", -1),
            s("path", {
              fill: "currentColor",
              d: "M10 19h4v-3h-4v3zM5 4v3h5v3h4V7h5V4H5zM3 14h18v-2H3v2z"
            }, null, -1)
          ])])) : x("", !0),
          b("undo") ? (a(), i("svg", {
            key: 10,
            class: g(Ye),
            onClick: C,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[11] || (m[11] = [
            s("title", null, "Undo (CTRL+Z)", -1),
            s("path", {
              fill: "currentColor",
              d: "M12.5 8c-2.65 0-5.05.99-6.9 2.6L2 7v9h9l-3.62-3.62c1.39-1.16 3.16-1.88 5.12-1.88c3.54 0 6.55 2.31 7.6 5.5l2.37-.78C21.08 11.03 17.15 8 12.5 8z"
            }, null, -1)
          ])])) : x("", !0),
          b("redo") ? (a(), i("svg", {
            key: 11,
            class: g(Ye),
            onClick: P,
            xmlns: "http://www.w3.org/2000/svg",
            width: "24",
            height: "24",
            viewBox: "0 0 24 24"
          }, [...m[12] || (m[12] = [
            s("title", null, "Redo (CTRL+SHIFT+Z)", -1),
            s("path", {
              fill: "currentColor",
              d: "M18.4 10.6C16.55 8.99 14.15 8 11.5 8c-4.65 0-8.58 3.03-9.96 7.22L3.9 16a8.002 8.002 0 0 1 7.6-5.5c1.95 0 3.73.72 5.12 1.88L13 16h9V7l-3.6 3.6z"
            }, null, -1)
          ])])) : x("", !0),
          O(w.$slots, "toolbarbuttons", {
            instance: Pe()?.exposed
          })
        ]),
        b("help") && e.helpUrl ? (a(), i("div", bv, [
          s("a", {
            title: "formatting help",
            target: "_blank",
            href: e.helpUrl,
            tabindex: "-1"
          }, [
            (a(), i("svg", {
              class: g(Ye),
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, [...m[13] || (m[13] = [
              s("path", {
                fill: "currentColor",
                d: "M11 18h2v-2h-2v2zm1-16C6.48 2 2 6.48 2 12s4.48 10 10 10s10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8s8 3.59 8 8s-3.59 8-8 8zm0-14c-2.21 0-4 1.79-4 4h2c0-1.1.9-2 2-2s2 .9 2 2c0 2-3 1.75-3 5h2c0-2.25 3-2.5 3-5c0-2.21-1.79-4-4-4z"
              }, null, -1)
            ])]))
          ], 8, wv)
        ])) : x("", !0)
      ])),
      s("div", xv, [
        s("textarea", {
          ref_key: "txt",
          ref: h,
          name: e.id,
          id: e.id,
          class: g(y.value),
          label: e.label,
          value: e.modelValue,
          rows: e.rows || 6,
          disabled: e.disabled,
          onInput: m[0] || (m[0] = (W) => k(W.target?.value || "")),
          onKeydown: tn(de, ["tab"])
        }, null, 42, kv)
      ]),
      f.value ? (a(), i("p", {
        key: 2,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, T(f.value), 9, Cv)) : e.help ? (a(), i("p", {
        key: 3,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, T(e.help), 9, $v)) : x("", !0),
      O(w.$slots, "footer", Ce({
        inputElement: h.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, w.$attrs))
    ]));
  }
}), Vv = {
  key: 0,
  class: "relative z-10 lg:hidden",
  role: "dialog",
  "aria-modal": "true"
}, Mv = { class: "fixed inset-0 flex" }, Sv = { class: "flex grow flex-col gap-y-5 overflow-y-auto bg-white dark:bg-black px-6 pb-2" }, Tv = { class: "hidden lg:fixed lg:inset-y-0 lg:z-10 lg:flex lg:w-72 lg:flex-col" }, Fv = { class: "flex grow flex-col gap-y-5 overflow-y-auto border-r border-gray-200 dark:border-gray-700 bg-white dark:bg-black px-6" }, Av = {
  class: /* @__PURE__ */ g(["sticky top-0 flex items-center gap-x-6 bg-white dark:bg-black px-4 py-4 shadow-sm sm:px-6 lg:hidden"])
}, Iv = /* @__PURE__ */ ue({
  __name: "SidebarLayout",
  setup(e, { expose: t }) {
    const { transition: l } = Gn(), n = A(!0), o = A(""), r = {
      entering: { cls: "transition-opacity ease-linear duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "transition-opacity ease-linear duration-300", from: "opacity-100", to: "opacity-0" }
    }, u = A(""), d = {
      entering: { cls: "transition ease-in-out duration-300 transform", from: "-translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transition ease-in-out duration-300 transform", from: "translate-x-0", to: "-translate-x-full" }
    }, f = A(""), v = {
      entering: { cls: "ease-in-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in-out duration-300", from: "opacity-100", to: "opacity-0" }
    };
    function p(y) {
      l(r, o, y), l(d, u, y), l(v, f, y), setTimeout(() => n.value = y, 300);
    }
    function $() {
      p(!0);
    }
    function b() {
      p(!1);
    }
    return t({ show: $, hide: b, toggle: p }), (y, h) => (a(), i("div", null, [
      n.value ? (a(), i("div", Vv, [
        s("div", {
          class: g(["fixed inset-0 bg-gray-900/80", o.value])
        }, null, 2),
        s("div", Mv, [
          s("div", {
            class: g(["relative mr-16 flex w-full max-w-xs flex-1", u.value])
          }, [
            s("div", {
              class: g(["absolute left-full top-0 flex w-16 justify-center pt-5", f.value])
            }, [
              s("button", {
                type: "button",
                onClick: b,
                class: "-m-2.5 p-2.5"
              }, [...h[0] || (h[0] = [
                s("span", { class: "sr-only" }, "Close sidebar", -1),
                s("svg", {
                  class: "h-6 w-6 text-white dark:text-black",
                  fill: "none",
                  viewBox: "0 0 24 24",
                  "stroke-width": "1.5",
                  stroke: "currentColor",
                  "aria-hidden": "true"
                }, [
                  s("path", {
                    "stroke-linecap": "round",
                    "stroke-linejoin": "round",
                    d: "M6 18L18 6M6 6l12 12"
                  })
                ], -1)
              ])])
            ], 2),
            s("div", Sv, [
              O(y.$slots, "default")
            ])
          ], 2)
        ])
      ])) : x("", !0),
      s("div", Tv, [
        s("div", Fv, [
          O(y.$slots, "default")
        ])
      ]),
      s("div", Av, [
        s("button", {
          type: "button",
          onClick: $,
          class: "-m-2.5 p-2.5 text-gray-700 dark:text-gray-200 lg:hidden"
        }, [...h[1] || (h[1] = [
          s("span", { class: "sr-only" }, "Open sidebar", -1),
          s("svg", {
            class: "h-6 w-6",
            fill: "none",
            viewBox: "0 0 24 24",
            "stroke-width": "1.5",
            stroke: "currentColor",
            "aria-hidden": "true"
          }, [
            s("path", {
              "stroke-linecap": "round",
              "stroke-linejoin": "round",
              d: "M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
            })
          ], -1)
        ])]),
        O(y.$slots, "mobiletitlebar")
      ])
    ]));
  }
}), Pv = Ro, Dv = _o, Bv = Jo, jv = Yo, Ov = Yn, Hv = li, Rv = ai, Ev = ri, Nv = ci, zv = fi, Uv = yi, qv = xi, Qv = $i, Kv = Pi, _v = iu, Zv = fu, Wv = Ds, Gv = Ps, Jv = Bs, Xv = ea, Yv = vu, em = wu, tm = Mu, lm = Pu, sm = Nu, nm = ad, am = Vd, om = jd, im = Rd, rm = Ed, um = Xd, dm = Yd, cm = hc, fm = Oc, vm = l0, mm = b0, gm = w0, pm = $0, ym = T0, hm = F0, bm = j0, wm = E0, xm = U0, km = q0, Cm = _0, $m = rf, Lm = vf, Vm = qf, Mm = Yf, Sm = ev, Tm = gv, Fm = Lv, Am = Iv, Im = {
  Alert: Pv,
  AlertSuccess: Dv,
  ErrorSummary: Bv,
  InputDescription: jv,
  Icon: Ov,
  Loading: Hv,
  OutlineButton: Rv,
  PrimaryButton: Ev,
  SecondaryButton: Nv,
  TextLink: zv,
  Breadcrumbs: Uv,
  Breadcrumb: qv,
  NavList: Qv,
  NavListItem: Kv,
  AutoQueryGrid: _v,
  SettingsIcons: Zv,
  FilterViews: Wv,
  FilterColumn: Gv,
  QueryPrefs: Jv,
  EnsureAccess: Xv,
  EnsureAccessDialog: Yv,
  TextInput: em,
  TextareaInput: tm,
  SelectInput: lm,
  CheckboxInput: sm,
  TagInput: nm,
  FileInput: am,
  Autocomplete: om,
  Combobox: im,
  DynamicInput: rm,
  LookupInput: um,
  AutoFormFields: dm,
  AutoForm: cm,
  AutoCreateForm: fm,
  AutoEditForm: vm,
  AutoViewForm: mm,
  ConfirmDelete: gm,
  FormLoading: pm,
  DataGrid: ym,
  CellFormat: hm,
  PreviewFormat: bm,
  HtmlFormat: wm,
  MarkupFormat: xm,
  MarkupModel: km,
  CloseButton: Cm,
  SlideOver: $m,
  ModalDialog: Lm,
  ModalLookup: Vm,
  Tabs: Mm,
  DarkModeToggle: Sm,
  SignIn: Tm,
  MarkdownInput: Fm,
  SidebarLayout: Am
}, _l = Im || {}, Om = {
  install(e) {
    Object.keys(_l).forEach((l) => {
      e.component(l, _l[l]);
    });
    function t(l) {
      const o = Object.keys(l).filter((r) => l[r]).map((r) => `${encodeURIComponent(r)}=${encodeURIComponent(l[r])}`).join("&");
      return o ? "?" + o : "./";
    }
    e.directive("href", function(l, n) {
      l.href = t(n.value), l.onclick = (o) => {
        o.preventDefault(), history.pushState(n.value, "", t(n.value));
      };
    });
  },
  component(e, t) {
    return e ? t ? N.components[e] = t : N.components[e] || _l[e] || null : null;
  }
};
export {
  Pv as Alert,
  Dv as AlertSuccess,
  fm as AutoCreateForm,
  vm as AutoEditForm,
  cm as AutoForm,
  dm as AutoFormFields,
  _v as AutoQueryGrid,
  mm as AutoViewForm,
  om as Autocomplete,
  qv as Breadcrumb,
  Uv as Breadcrumbs,
  hm as CellFormat,
  sm as CheckboxInput,
  Cm as CloseButton,
  im as Combobox,
  Im as Components,
  gm as ConfirmDelete,
  Sm as DarkModeToggle,
  ym as DataGrid,
  rm as DynamicInput,
  Xv as EnsureAccess,
  Yv as EnsureAccessDialog,
  Bv as ErrorSummary,
  am as FileInput,
  Gv as FilterColumn,
  Wv as FilterViews,
  pm as FormLoading,
  wm as HtmlFormat,
  Ov as Icon,
  jv as InputDescription,
  Hv as Loading,
  um as LookupInput,
  Fm as MarkdownInput,
  xm as MarkupFormat,
  km as MarkupModel,
  Ra as MetadataApp,
  Lm as ModalDialog,
  Vm as ModalLookup,
  Qv as NavList,
  Kv as NavListItem,
  Rv as OutlineButton,
  bm as PreviewFormat,
  Ev as PrimaryButton,
  Jv as QueryPrefs,
  Nv as SecondaryButton,
  lm as SelectInput,
  Zv as SettingsIcons,
  Am as SidebarLayout,
  Tm as SignIn,
  $m as SlideOver,
  Mm as Tabs,
  nm as TagInput,
  em as TextInput,
  zv as TextLink,
  tm as TextareaInput,
  jm as css,
  Om as default,
  Xa as initMetadata,
  Is as useAuth,
  fl as useClient,
  Ot as useConfig,
  Ha as useFiles,
  Bm as useFormatters,
  vt as useMetadata,
  Gn as useUtils
};
