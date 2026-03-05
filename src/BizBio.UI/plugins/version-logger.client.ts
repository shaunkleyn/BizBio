export default defineNuxtPlugin(() => {
    fetch('/build-info.json')
        .then(r => r.ok ? r.json() : null)
        .then((info: { version?: string } | null) => {
            if (info?.version) {
                console.log(
                    `%c[BizBio] ${info.version}`,
                    'color:#7c3aed;font-weight:bold;font-size:11px'
                )
            }
        })
        .catch(() => { /* non-critical */ })
})
