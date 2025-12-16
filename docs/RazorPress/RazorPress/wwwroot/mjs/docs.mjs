import { $$, $1, on, leftPart, map } from "@servicestack/client"
//change #hash on scroll
const headings = $$('h2[id],h3[id]')

function select(id) {
    $$(`.docmap .active`).forEach(el => el.classList.remove('active'))
    map($1(`.docmap [data-id='${id}']`), el => {
        el.classList.add('active')
        map(el.closest('.group'), el => el.classList.add('active'))
    })
}

on(document, {
    scroll(e) {
        headings.forEach(ha => {
            const rect = ha.getBoundingClientRect()
            if (rect.top > 0 && rect.top < 150) {
                const location = leftPart(window.location.toString(), '#')
                history.replaceState(null, null, location + '#' + ha.id)
                select(ha.id)
            }
        })
    }
})
if (location.hash) {
    select(location.hash.substring(1))
}

//scroll menu item into view
function isInView(el) {
    const box = el.getBoundingClientRect();
    return box.top < window.innerHeight && box.bottom >= 0;
}

const active = $1('.sidebar .active')
if (active && !isInView(active)) {
    (active.parentElement.previousElementSibling || active.parentElement.parentElement || active).scrollIntoView()
}

/* used in :::copy */
globalThis.copy = function(e) {
    e.classList.add('copying')
    let $el = document.createElement("textarea")
    let text = (e.querySelector('code') || e.querySelector('p')).innerHTML
    $el.innerHTML = text
    document.body.appendChild($el)
    $el.select()
    document.execCommand("copy")
    document.body.removeChild($el)
    setTimeout(() => e.classList.remove('copying'), 3000)
}

/* used in :::sh CopyContainerRenderer */
globalThis.shellCopy = async function(e) {
    // Get the text content from the code element
    const codeEl = e.querySelector('code')
    const textToCopy = codeEl?.textContent?.trim() || ""

    // Find the button element and icons
    const btn = e.querySelector('.shell-copy-btn')
    if (!btn) return

    const copiedIcon = btn.querySelector('.copied')
    const nocopyIcon = btn.querySelector('.nocopy')

    try {
        // Use modern Clipboard API
        await navigator.clipboard.writeText(textToCopy)

        // Show checkmark, hide copy icon
        if (copiedIcon) copiedIcon.style.display = 'block'
        if (nocopyIcon) nocopyIcon.style.display = 'none'

        // Add copied state to button
        btn.classList.remove('bg-white', 'dark:bg-slate-800', 'text-slate-700', 'dark:text-slate-300', 'border-slate-300', 'dark:border-slate-600', 'hover:bg-slate-50', 'dark:hover:bg-slate-700', 'hover:border-slate-400', 'dark:hover:border-slate-500')
        btn.classList.add('bg-green-100', 'dark:bg-green-900/30', 'text-green-700', 'dark:text-green-400', 'border-green-300', 'dark:border-green-700')

        // Reset after 2 seconds
        setTimeout(() => {
            // Hide checkmark, show copy icon
            if (copiedIcon) copiedIcon.style.display = 'none'
            if (nocopyIcon) nocopyIcon.style.display = 'block'

            btn.classList.remove('bg-green-100', 'dark:bg-green-900/30', 'text-green-700', 'dark:text-green-400', 'border-green-300', 'dark:border-green-700')
            btn.classList.add('bg-white', 'dark:bg-slate-800', 'text-slate-700', 'dark:text-slate-300', 'border-slate-300', 'dark:border-slate-600', 'hover:bg-slate-50', 'dark:hover:bg-slate-700', 'hover:border-slate-400', 'dark:hover:border-slate-500')
        }, 2000)
    } catch (err) {
        // Fallback for older browsers
        const $el = document.createElement("textarea")
        $el.value = textToCopy
        $el.style.position = "fixed"
        $el.style.opacity = "0"
        document.body.appendChild($el)
        $el.select()
        document.execCommand("copy")
        document.body.removeChild($el)

        // Show checkmark, hide copy icon
        if (copiedIcon) copiedIcon.style.display = 'block'
        if (nocopyIcon) nocopyIcon.style.display = 'none'

        // Add copied state to button
        btn.classList.remove('bg-white', 'dark:bg-slate-800', 'text-slate-700', 'dark:text-slate-300', 'border-slate-300', 'dark:border-slate-600', 'hover:bg-slate-50', 'dark:hover:bg-slate-700', 'hover:border-slate-400', 'dark:hover:border-slate-500')
        btn.classList.add('bg-green-100', 'dark:bg-green-900/30', 'text-green-700', 'dark:text-green-400', 'border-green-300', 'dark:border-green-700')

        // Reset after 2 seconds
        setTimeout(() => {
            // Hide checkmark, show copy icon
            if (copiedIcon) copiedIcon.style.display = 'none'
            if (nocopyIcon) nocopyIcon.style.display = 'block'

            btn.classList.remove('bg-green-100', 'dark:bg-green-900/30', 'text-green-700', 'dark:text-green-400', 'border-green-300', 'dark:border-green-700')
            btn.classList.add('bg-white', 'dark:bg-slate-800', 'text-slate-700', 'dark:text-slate-300', 'border-slate-300', 'dark:border-slate-600', 'hover:bg-slate-50', 'dark:hover:bg-slate-700', 'hover:border-slate-400', 'dark:hover:border-slate-500')
        }, 2000)
    }
}
