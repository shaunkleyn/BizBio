export const useImageOptimization = () => {
  /**
   * Optimizes an image by resizing and compressing it
   * @param file - The original image file
   * @param options - Optimization options
   * @returns Promise with the optimized file and preview URL
   */
  const optimizeImage = async (
    file: File,
    options: {
      maxWidth?: number
      maxHeight?: number
      quality?: number
      outputFormat?: 'jpeg' | 'png' | 'webp'
    } = {}
  ): Promise<{ file: File; previewUrl: string; originalSize: number; optimizedSize: number }> => {
    const {
      maxWidth = 1200,
      maxHeight = 1200,
      quality = 0.85,
      outputFormat = 'jpeg'
    } = options

    return new Promise((resolve, reject) => {
      const reader = new FileReader()

      reader.onerror = () => reject(new Error('Failed to read file'))

      reader.onload = (e) => {
        const img = new Image()

        img.onerror = () => reject(new Error('Failed to load image'))

        img.onload = () => {
          // Calculate new dimensions while maintaining aspect ratio
          let width = img.width
          let height = img.height

          if (width > maxWidth || height > maxHeight) {
            const aspectRatio = width / height

            if (width > height) {
              width = maxWidth
              height = width / aspectRatio
            } else {
              height = maxHeight
              width = height * aspectRatio
            }
          }

          // Create canvas and draw resized image
          const canvas = document.createElement('canvas')
          canvas.width = width
          canvas.height = height

          const ctx = canvas.getContext('2d')
          if (!ctx) {
            reject(new Error('Failed to get canvas context'))
            return
          }

          // Enable image smoothing for better quality
          ctx.imageSmoothingEnabled = true
          ctx.imageSmoothingQuality = 'high'

          // Draw image on canvas
          ctx.drawImage(img, 0, 0, width, height)

          // Determine output format and mime type
          const mimeType = outputFormat === 'png' ? 'image/png' :
                           outputFormat === 'webp' ? 'image/webp' :
                           'image/jpeg'

          const extension = outputFormat === 'png' ? '.png' :
                            outputFormat === 'webp' ? '.webp' :
                            '.jpg'

          // Convert canvas to blob
          canvas.toBlob(
            (blob) => {
              if (!blob) {
                reject(new Error('Failed to create blob'))
                return
              }

              // Create new file from blob
              const originalName = file.name.replace(/\.[^/.]+$/, '')
              const optimizedFile = new File(
                [blob],
                `${originalName}${extension}`,
                { type: mimeType }
              )

              // Create preview URL
              const previewUrl = URL.createObjectURL(blob)

              resolve({
                file: optimizedFile,
                previewUrl,
                originalSize: file.size,
                optimizedSize: blob.size
              })
            },
            mimeType,
            quality
          )
        }

        img.src = e.target?.result as string
      }

      reader.readAsDataURL(file)
    })
  }

  /**
   * Optimizes an image for logo use (smaller size, square crop)
   */
  const optimizeLogo = async (file: File): Promise<{ file: File; previewUrl: string }> => {
    const result = await optimizeImage(file, {
      maxWidth: 400,
      maxHeight: 400,
      quality: 0.9,
      outputFormat: 'png'
    })

    return {
      file: result.file,
      previewUrl: result.previewUrl
    }
  }

  /**
   * Optimizes an image for menu items
   */
  const optimizeMenuImage = async (file: File): Promise<{ file: File; previewUrl: string }> => {
    const result = await optimizeImage(file, {
      maxWidth: 800,
      maxHeight: 800,
      quality: 0.85,
      outputFormat: 'jpeg'
    })

    return {
      file: result.file,
      previewUrl: result.previewUrl
    }
  }

  /**
   * Formats file size for display
   */
  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 Bytes'

    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i]
  }

  /**
   * Calculates compression percentage
   */
  const calculateCompressionPercent = (originalSize: number, optimizedSize: number): number => {
    return Math.round(((originalSize - optimizedSize) / originalSize) * 100)
  }

  return {
    optimizeImage,
    optimizeLogo,
    optimizeMenuImage,
    formatFileSize,
    calculateCompressionPercent
  }
}
