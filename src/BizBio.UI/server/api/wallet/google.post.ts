import { defineEventHandler, readBody, createError } from 'h3'
import { createPrivateKey } from 'crypto'
import pkg from 'jsonwebtoken'
const { sign } = pkg

interface CardData {
    name: string
    title?: string
    company?: string
    email?: string
    phone?: string
    website?: string
    photo?: string
    address?: string
    bio?: string
    profileUrl?: string
    // Wallet appearance customisation
    walletBg?: string      // hex background colour, e.g. "#1a1a2e"
    walletTitle?: string   // card title shown in the wallet, e.g. "BizBio Business Card"
    walletLogo?: string    // short text logo shown in top-left if no logo image
    walletLogoUrl?: string     // absolute URL to a logo image (HTTPS, ≤ 1 MB, ~512×512 px)
    walletWideLogoUrl?: string // absolute URL to a wide logo (HTTPS, ≤ 1 MB, ~320×120 px ideal)
    walletHeroUrl?: string     // absolute URL to a hero/banner image (HTTPS, 1032×336 px ideal)
    walletQr?: string          // value to encode in the QR code (defaults to profile URL)
}

export default defineEventHandler(async (event) => {
    const body: CardData = await readBody(event)
    const config = useRuntimeConfig()

    const issuerId = config.googleWalletIssuerId as string
    const serviceAccountEmail = config.googleWalletServiceAccountEmail as string
    const rawKey = (config.googleWalletServiceAccountKey as string)?.replace(/\\n/g, '\n')

    if (!issuerId || !serviceAccountEmail || !rawKey) {
        throw createError({
            statusCode: 500,
            message: 'Google Wallet is not configured on this server.',
        })
    }

    let privateKey: ReturnType<typeof createPrivateKey>
    try {
        privateKey = createPrivateKey(rawKey)
    } catch {
        throw createError({
            statusCode: 500,
            message: 'Invalid Google Wallet private key. Check GOOGLE_WALLET_SERVICE_ACCOUNT_KEY in your .env file.',
        })
    }

    if (!body.name) {
        throw createError({ statusCode: 400, message: 'Card name is required.' })
    }

    const classId = `${issuerId}.bizbio_business_card`

    // Unique object ID per card instance
    const objectSuffix = body.name
        .toLowerCase()
        .replace(/[^a-z0-9]/g, '_')
        .replace(/_+/g, '_')
    const objectId = `${issuerId}.${objectSuffix}_${Date.now()}`

    // Build text modules for contact details
    const textModulesData = []
    if (body.company) textModulesData.push({ id: 'company', header: 'COMPANY', body: body.company })
    if (body.phone) textModulesData.push({ id: 'phone', header: 'PHONE', body: body.phone })
    if (body.email) textModulesData.push({ id: 'email', header: 'EMAIL', body: body.email })
    if (body.address) textModulesData.push({ id: 'address', header: 'ADDRESS', body: body.address })
    if (body.bio) textModulesData.push({ id: 'bio', header: 'ABOUT', body: body.bio })

    // Build link modules
    const uris = []
    if (body.website) uris.push({ uri: body.website, description: 'Website', id: 'website' })
    if (body.profileUrl) uris.push({ uri: body.profileUrl, description: 'Digital Business Card', id: 'profile' })

    // Build the Generic Pass object
    const genericObject: Record<string, unknown> = {
        id: objectId,
        classId: classId,
        state: 'ACTIVE',
        hexBackgroundColor: body.walletBg || '#1a1a2e',
        cardTitle: {
            defaultValue: { language: 'en', value: body.walletTitle || 'BizBio Business Card' },
        },
        header: {
            defaultValue: { language: 'en', value: body.name },
        },
    }

    if (body.title) {
        genericObject.subheader = {
            defaultValue: { language: 'en', value: body.title },
        }
    }

    // Square logo (top-left of card)
    if (body.walletLogoUrl) {
        genericObject.logo = {
            sourceUri: { uri: body.walletLogoUrl },
            contentDescription: {
                defaultValue: { language: 'en', value: body.walletLogo || body.company || body.name },
            },
        }
    }

    // Wide logo (shown in the barcode strip header — ~320×120 px)
    if (body.walletWideLogoUrl) {
        genericObject.wideLogo = {
            sourceUri: { uri: body.walletWideLogoUrl },
            contentDescription: {
                defaultValue: { language: 'en', value: body.walletLogo || body.company || body.name },
            },
        }
    }

    // Hero/banner image — walletHeroUrl wins over profile photo
    const heroUrl = body.walletHeroUrl || body.photo || null
    if (heroUrl) {
        genericObject.heroImage = {
            sourceUri: { uri: heroUrl },
            contentDescription: {
                defaultValue: { language: 'en', value: body.name },
            },
        }
    }

    // QR code — encodes the profile URL by default, or a custom value from data.json
    const qrValue = body.walletQr || body.profileUrl || null
    if (qrValue) {
        genericObject.barcode = {
            type: 'QR_CODE',
            value: qrValue,
            alternateText: body.website || qrValue,
            renderEncoding: 'UTF_8',
        }
    }

    if (textModulesData.length > 0) {
        genericObject.textModulesData = textModulesData
    }

    if (uris.length > 0) {
        genericObject.linksModuleData = { uris }
    }

    // The JWT payload — including the class definition so it's auto-created on first use
    const claims = {
        iss: serviceAccountEmail,
        aud: 'google',
        typ: 'savetowallet',
        iat: Math.floor(Date.now() / 1000),
        payload: {
            genericClasses: [{ id: classId }],
            genericObjects: [genericObject],
        },
    }

    const token = sign(claims, privateKey, { algorithm: 'RS256' })

    return {
        url: `https://pay.google.com/gp/v/save/${token}`,
    }
})
