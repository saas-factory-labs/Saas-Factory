import { ref, nextTick, onMounted } from 'vue'

const TypesenseDialog = {
    template:`<div :style="{ 
         display: open ? 'flex' : 'none', 
         position: 'fixed', 
         top: 0, 
         left: 0, 
         right: 0, 
         bottom: 0, 
         zIndex: 200, 
         alignItems: 'flex-start', 
         justifyContent: 'center', 
         paddingTop: '20vh',
         background: '#0f172a'
       }"
       @click="$emit('hide')">
    <div @click.stop="" 
         style="position: relative; width: 90%; max-width: 640px; max-height: 70vh; 
                display: flex; flex-direction: column; background: #1e293b; 
                border-radius: 12px; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.5); overflow: hidden;">
      <div style="display: flex; flex-direction: column; max-height: 70vh;">
        <div style="display: flex; align-items: center; padding: 0 16px; background: #0f172a; border-bottom: 1px solid #334155;">
          <label style="display: flex; align-items: center; padding: 12px 0;" for="docsearch-input">
            <svg style="width: 20px; height: 20px; color: #64748b;" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
          </label>
          <input id="docsearch-input" v-model="query" @keyup="search"
                style="flex: 1; padding: 12px 12px; font-size: 16px; border: none; outline: none; background: transparent; color: #e2e8f0;"
                aria-autocomplete="list" aria-labelledby="docsearch-label" autocomplete="off" autocorrect="off" autocapitalize="off" 
                spellcheck="false" placeholder="Search documentation..." maxlength="64" type="search" enterkeyhint="go"
                @focus="selectedIndex=1" @blur="selectedIndex=-1" @keydown="onKeyDown">
          <div style="display: flex; align-items: center;">
            <button @click="$emit('hide')" 
                    style="padding: 4px 8px; background: #334155; border: none; 
                           border-radius: 4px; cursor: pointer; font-size: 12px; color: #94a3b8; font-weight: 500;">
              ESC
            </button>
          </div>
        </div>
        <div v-if="results.allItems.length" 
             style="padding: 8px; overflow-y: auto; max-height: 60vh; background: #1e293b;">
          <div v-for="g in results.groups" :key="g.group" style="margin-bottom: 16px;">
            <h3 style="margin: 0 0 8px 8px; font-size: 11px; color: #64748b; text-transform: uppercase; font-weight: 600; letter-spacing: 0.05em;" v-html="g.group"></h3>
            <div v-for="result in g.items" :key="result.id" :aria-selected="result.id == selectedIndex"
                 style="border-radius: 6px; background: #0f172a; margin-bottom: 2px; padding: 10px 12px; display: flex; cursor: pointer; transition: background 0.15s;"
                 @mouseover="onHover(result.id)" @click="go(result.url)">              
              <div style="min-width: min-content; margin-right: 12px; display: flex; align-items: flex-start; padding-top: 2px;">
                <svg v-if="result.type=='doc'" style="width: 18px; height: 18px; color: #64748b;" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
                <svg v-else-if="result.type=='content'" style="width: 18px; height: 18px; color: #64748b;" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14"></path></svg>
                <svg v-else style="width: 18px; height: 18px; color: #64748b;" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14"></path></svg>
              </div>
              <div style="overflow: hidden; flex: 1;">
                <h4 style="margin: 0 0 4px 0;"><a style="font-size: 14px; color: #e2e8f0; text-decoration: none; font-weight: 500; display: block;" :href="result.url" v-html="result.titleHtml"></a></h4>
                <div style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; font-size: 13px; color: #64748b;" v-html="result.snippetHtml"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>`,
    emits: ['hide'],
    props: { open: Boolean },
    setup(props, { emit }) {

        const results = ref({ groups:[], allItems:[] })
        const query = ref("")

        let lastQuery = ""
        let timeout = null
        function search (txt) {
            if (!query.value) {
                results.value = { groups:[], allItems:[] }
                return
            }
            timeout = setTimeout(() => {
                if (timeout != null) {
                    if (lastQuery === query.value) return
                    lastQuery = query.value
                    clearTimeout(timeout)
                    // typesense API reference: https://typesense.org/docs/29.0/api/documents.html#search
                    fetch('https://typesense-production-cfa5.up.railway.app/collections/saas_factory_docs/documents/search?q='
                        + encodeURIComponent(query.value)
                        + '&query_by=hierarchy.lvl0,hierarchy.lvl1,content,hierarchy.lvl2,hierarchy.lvl3&group_by=hierarchy.lvl0', {
                        headers: {
                            // Search only API key for Typesense - create with fixed value parameter
                            'x-typesense-api-key': 'saas-factory-docs-search-23859'
                        }
                    }).then(res => {
                        res.json().then(data => {
                            selectedIndex.value = 1
                            let idx = 0
                            const groups = {}
                            const meta = { groups:[], allItems:[] }
                            // console.log(data)

                            data.grouped_hits.forEach((gh) => {
                                let groupName = gh.group_key[0]
                                meta.groups.push({ group: groupName })
                                let group = groups[groupName] ?? (groups[groupName] = [])
                                gh.hits.forEach((hit) => {
                                    let doc = hit.document
                                    let highlight = hit.highlights.length > 0 ? hit.highlights[0] : null
                                    // Convert GitHub Pages URL to relative path
                                    let url = new URL(doc.url)
                                    let pathname = url.pathname
                                    // Strip /Saas-Factory/docs/ prefix from GitHub Pages URLs for localhost
                                    if (pathname.startsWith('/Saas-Factory/docs/')) {
                                        pathname = pathname.substring('/Saas-Factory/docs'.length)
                                    }
                                    // Strip numeric prefixes from file names (e.g., /development/03-creating -> /development/creating)
                                    pathname = pathname.replace(/\/(\d+-)([^/]+)/g, '/$2')
                                    
                                    let item = {
                                        id: ++idx,
                                        titleHtml: clean(doc.hierarchy.lvl3 ?? doc.hierarchy.lvl2 ?? doc.hierarchy.lvl1 ?? doc.hierarchy.lvl0),
                                        snippetHtml: clean(highlight?.snippet),
                                        // search result type for icon
                                        type: highlight?.field === 'content' ? 'content' : 'heading',
                                        url: pathname + (url.hash || '')
                                    };
                                    let titleOnly = stripHtml(item.titleHtml)
                                    if (titleOnly === groupName) {
                                        item.type = 'doc'
                                    }
                                    if (titleOnly === stripHtml(item.snippetHtml)) {
                                        item.snippetHtml = ""
                                    }
                                    group.push(item)
                                })
                            })

                            meta.groups.forEach((g) => {
                                g.items = groups[g.group] ?? []
                                g.items.forEach((item) => {
                                    meta.allItems.push(item)
                                })
                            })

                            //console.log(meta)
                            results.value = meta
                        })
                    })
                }
            }, 200)
        }

        let selectedIndex = ref(1)
        /** @param {number} index */
        const onHover = (index) => selectedIndex.value = index
        
        /** @param {string} url */
        function go(url) {
            emit('hide')
            location.href = url
        }

        /** @param {number} id
         *  @param {number} step */
        const next = (id, step) => {
            const meta = results.value
            const pos = meta.allItems.findIndex((x) => x.id === id)
            if (pos === -1)
                return meta.allItems[0]
            const nextPos = (pos + step) % meta.allItems.length
            return nextPos >= 0 ? meta.allItems[nextPos] : meta.allItems[meta.allItems.length + nextPos]
        }

        let ScrollCounter = 0

        /** @param {KeyboardEvent} e */
        function onKeyDown(e) {
            const meta = results.value
            if (!meta || meta.allItems.length === 0) return
            if (e.code === 'ArrowDown' || e.code === 'ArrowUp' || e.code === 'Home' || e.code === 'End') {
                selectedIndex.value = e.code === 'Home'
                    ? meta.allItems[0]?.id
                    : e.code === 'End'
                        ? meta.allItems[meta.allItems.length-1]?.id
                        : next(selectedIndex.value, e.code === 'ArrowUp' ? -1 : 1).id
                nextTick(() => {
                    let el = document.querySelector('[aria-selected=true]'),
                        elGroup = el?.closest('.group-result'),
                        elParent = elGroup?.closest('.group-results')

                    ScrollCounter++
                    let counter = ScrollCounter

                    if (el && elGroup && elParent) {
                        if (el === elGroup.firstElementChild?.nextElementSibling && elGroup === elParent.firstElementChild) {
                            //console.log('scrollTop', 0)
                            elParent.scrollTo({ top: 0, left: 0 });
                        } else if (el === elGroup.lastElementChild && elGroup === elParent.lastElementChild) {
                            //console.log('scrollEnd', elParent.scrollHeight)
                            elParent.scrollTo({ top: elParent.scrollHeight, left: 0 })
                        } else {
                            if (typeof IntersectionObserver != 'undefined') {
                                let observer = new IntersectionObserver((entries) => {
                                    if (entries[0].intersectionRatio <= 0) {
                                        //console.log('el.scrollIntoView()', counter, ScrollCounter)
                                        if (counter === ScrollCounter) el.scrollIntoView()
                                    }
                                    observer.disconnect()
                                })
                                observer.observe(el)
                            }
                        }
                    }
                })
                e.preventDefault()
            } else if (e.code === 'Enter') {
                let match = meta.allItems.find((x) => x.id === selectedIndex.value)
                if (match) {
                    go(match.url)
                    e.preventDefault()
                }
            }
        }

        /** @param {string} s */
        function stripHtml(s) {
            return s && s.replace(/<[^>]*>?/gm, '')
        }

        /** @param {string} s */
        function clean(s) {
            return s?.replace(/&ZeroWidthSpace/g,'')
        }

        return { results, query, selectedIndex, search, onHover, go, onKeyDown }
    }
}

export default {
    components: {
        TypesenseDialog,
    },
    template:`<div>
        <TypesenseDialog :open="openSearch" @hide="hideSearch" />
        <div style="display: flex; gap: 8px;">
            <button @click="showSearch" class="search-button"
                    :style="{ 
                        display: 'flex', 
                        alignItems: 'center', 
                        gap: '4px', 
                        padding: '8px 12px',
                        background: isDark ? '#1e293b' : '#f3f4f6',
                        border: isDark ? '2px solid #334155' : '2px solid #e5e7eb',
                        borderRadius: '9999px',
                        cursor: 'pointer',
                        color: isDark ? '#94a3b8' : '#9ca3af'
                    }"
                    @mouseover="e => { e.currentTarget.style.background = isDark ? '#334155' : '#fff'; e.currentTarget.style.borderColor = '#10b981'; e.currentTarget.style.color = isDark ? '#e2e8f0' : '#4b5563'; }"
                    @mouseout="e => { e.currentTarget.style.background = isDark ? '#1e293b' : '#f3f4f6'; e.currentTarget.style.borderColor = isDark ? '#334155' : '#e5e7eb'; e.currentTarget.style.color = isDark ? '#94a3b8' : '#9ca3af'; }">
                <svg style="width: 28px; height: 28px; margin-right: 4px;" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16l2.879-2.879m0 0a3 3 0 104.243-4.242 3 3 0 00-4.243 4.242zM21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span style="font-size: 18px; margin-right: 4px;">Search</span>
                <span :style="{ 
                    opacity: 1, 
                    color: isDark ? '#94a3b8' : '#9ca3af', 
                    fontSize: '14px', 
                    padding: '2px 6px',
                    border: isDark ? '1px solid #334155' : '1px solid #d1d5db',
                    borderRadius: '4px'
                }">
                    <kbd style="font-family: sans-serif;">/</kbd>
                </span>
            </button>
        </div>
    </div>
    `,
    setup() {
        const openSearch = ref(false)
        const isDark = ref(document.documentElement.classList.contains('dark'))
        function showSearch() {
            console.log('showSearch', openSearch.value, document.querySelector('#docsearch-input'))
            openSearch.value = true
            nextTick(() => {
                /** @type {HTMLInputElement} */
                const el = document.querySelector('#docsearch-input')
                el?.select();
                el?.focus();
            })
        }
        const hideSearch = () => openSearch.value = false
        /** @param {KeyboardEvent} e */
        function onKeyDown(e) {
            if (e.code === 'Escape') {
                hideSearch();
            }
            else if (e.target.tagName !== 'INPUT') {
                if (e.code === 'Slash' || (e.ctrlKey && e.code === 'KeyK')) {
                    showSearch();
                    e.preventDefault();
                }
            }
        }

        onMounted(() => {
            window.addEventListener('keydown', onKeyDown)
        })

        return {
            openSearch,
            showSearch,
            hideSearch,
            onKeyDown,
            isDark,
        }
    }
}
