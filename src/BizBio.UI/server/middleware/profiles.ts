import { defineEventHandler, getRequestURL, setResponseHeader, sendRedirect } from 'h3'
import { readFile, stat } from 'fs/promises'
import { join } from 'path'

const SKIP_PREFIXES = ['/_nuxt', '/api/', '/__nuxt', '/@', '/favicon']

const MIME_TYPES: Record<string, string> = {
    '.html': 'text/html; charset=utf-8',
    '.js': 'application/javascript; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.json': 'application/json; charset=utf-8',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg',
    '.png': 'image/png',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon',
    '.webp': 'image/webp',
    '.vcf': 'text/vcard',
    '.pkpass': 'application/vnd.apple.pkpass',
}

function getMimeType(filePath: string): string {
    const ext = filePath.substring(filePath.lastIndexOf('.')).toLowerCase()
    return MIME_TYPES[ext] ?? 'application/octet-stream'
}

export default defineEventHandler(async (event) => {
    const pathname = getRequestURL(event).pathname

    // Let Nuxt / API routes handle their own paths
    if (SKIP_PREFIXES.some(prefix => pathname.startsWith(prefix))) return

    const profilesDir = join(process.cwd(), 'profiles')
    let filePath = join(profilesDir, pathname)

    try {
        const stats = await stat(filePath)
        if (stats.isDirectory()) {
            // Redirect to trailing-slash so relative URLs (data.json, photo.jpg)
            // resolve correctly within the profile directory.
            if (!pathname.endsWith('/')) {
                return sendRedirect(event, pathname + '/', 301)
            }
            filePath = join(filePath, 'index.html')
            await stat(filePath) // confirm index.html exists
        }
    } catch {
        // No matching file in profiles — let Nuxt handle the request
        return
    }

    try {
        const content = await readFile(filePath)
        setResponseHeader(event, 'Content-Type', getMimeType(filePath))
        setResponseHeader(event, 'Cache-Control', 'no-cache')
        return content
    } catch {
        return
    }
})
