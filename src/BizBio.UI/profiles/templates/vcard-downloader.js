/**
 * Main function to set up VCF download and sharing logic for a contact page.
 *
 * @param {object} options - Configuration object.
 * @param {string} options.clientName - The name of the client (e.g., "Six Door Innovations").
 * @param {string} options.vcfFileName - The name of the VCF file (e.g., "Six_Door_Innovations.vcf").
 * @param {string} options.clientUrl - The public URL for the client's profile (e.g., "https://bizbio.co.za/sixdoor").
 * @param {string} options.downloadBtnId - The ID of the VCF download button element (e.g., "downloadContact").
 * @param {string} options.shareContactBtnId - The ID of the 'Share VCF' button element (e.g., "shareContact").
 * @param {string} options.shareLinkBtnId - The ID of the 'Share Link' button element (e.g., "shareLink").
 */
function initializeContactSharing(options) {
    const {
        clientName,
        vcfFileName,
        clientUrl,
        downloadBtnId,
        shareContactBtnId,
        shareLinkBtnId
    } = options;

    const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

    // --- 1. Download Button Logic (iOS Adjustment) ---
    const downloadBtn = document.getElementById(downloadBtnId);
    if (downloadBtn) {
        if (isIOS) {
            // iOS Safari prefers a direct link; set the href directly to the file path
            downloadBtn.setAttribute("href", vcfFileName);
            downloadBtn.addEventListener("click", () => {
                console.log("iOS: Opening VCF in Contacts app via direct link.");
            });
        }
        // For other browsers, the standard HTML `download` attribute on the anchor tag
        // is usually sufficient and superior, but if you need a fallback:
        // downloadBtn.setAttribute("href", vcfFileName);
        // downloadBtn.setAttribute("download", vcfFileName);
    } else {
        console.warn(`Download button with ID '${downloadBtnId}' not found.`);
    }

    // --- 2. Share Contact Button Logic (Web Share API with file) ---
    const shareContactBtn = document.getElementById(shareContactBtnId);
    if (shareContactBtn) {
        shareContactBtn.addEventListener("click", async () => {
            try {
                const response = await fetch(vcfFileName);
                const blob = await response.blob();
                const file = new File([blob], vcfFileName, { type: "text/vcard" });

                if (navigator.canShare && navigator.canShare({ files: [file] })) {
                    await navigator.share({
                        title: `${clientName} Contact`,
                        text: `Here are the contact details for ${clientName}!`,
                        files: [file]
                    });
                } else {
                    alert("Sharing VCF files is not supported on this device/browser. Please use the Save Contact button.");
                }
            } catch (err) {
                console.error("VCF Sharing failed:", err);
                alert("VCF Sharing failed. Please use the Save Contact button instead.");
            }
        });
    } else {
        console.warn(`Share Contact button with ID '${shareContactBtnId}' not found.`);
    }

    // --- 3. Share Link Button Logic (Web Share API with URL) ---
    const shareLinkBtn = document.getElementById(shareLinkBtnId);
    if (shareLinkBtn) {
        shareLinkBtn.addEventListener("click", async (event) => {
            event.preventDefault(); // Stop navigation to "#" if using <a href="#">
            try {
                if (navigator.share) {
                    await navigator.share({
                        title: `${clientName} Profile`,
                        text: `Check out ${clientName} on BizBio!`,
                        url: clientUrl
                    });
                } else {
                    alert("Your browser does not support sharing. Please copy the link manually.");
                    // Optional: Copy link to clipboard fallback here
                }
            } catch (err) {
                console.error("Link Sharing failed:", err);
            }
        });
    } else {
        console.warn(`Share Link button with ID '${shareLinkBtnId}' not found.`);
    }
} 