export const useCsvImport = () => {
  // Parse CSV file to array of objects
  const parseCSV = (csvText: string): any[] => {
    const lines = csvText.split('\n').map(line => line.trim()).filter(line => line.length > 0)

    if (lines.length === 0) {
      throw new Error('CSV file is empty')
    }

    // Get headers from first line
    const headers = lines[0].split(',').map(h => h.trim().replace(/^["']|["']$/g, ''))

    // Parse data rows
    const data: any[] = []
    for (let i = 1; i < lines.length; i++) {
      const values = parseCSVLine(lines[i])

      if (values.length !== headers.length) {
        console.warn(`Row ${i + 1} has ${values.length} columns, expected ${headers.length}. Skipping.`)
        continue
      }

      const row: any = {}
      headers.forEach((header, index) => {
        row[header] = values[index]
      })
      data.push(row)
    }

    return data
  }

  // Parse a single CSV line handling quoted values
  const parseCSVLine = (line: string): string[] => {
    const result: string[] = []
    let current = ''
    let inQuotes = false

    for (let i = 0; i < line.length; i++) {
      const char = line[i]
      const nextChar = line[i + 1]

      if (char === '"') {
        if (inQuotes && nextChar === '"') {
          // Escaped quote
          current += '"'
          i++ // Skip next quote
        } else {
          // Toggle quote state
          inQuotes = !inQuotes
        }
      } else if (char === ',' && !inQuotes) {
        // End of field
        result.push(current.trim())
        current = ''
      } else {
        current += char
      }
    }

    // Add last field
    result.push(current.trim())

    return result
  }

  // Validate menu items from CSV
  const validateMenuItems = (data: any[], categories: any[]): { valid: any[], errors: any[] } => {
    const valid: any[] = []
    const errors: any[] = []

    const categoryMap = new Map(categories.map(c => [c.name.toLowerCase(), c.id]))

    data.forEach((row, index) => {
      const rowErrors: string[] = []
      const rowNumber = index + 2 // +2 because index starts at 0 and we skip header

      // Required fields validation
      if (!row.name || row.name.trim() === '') {
        rowErrors.push('Name is required')
      }

      if (!row.category || row.category.trim() === '') {
        rowErrors.push('Category is required')
      } else {
        // Check if category exists
        const categoryId = categoryMap.get(row.category.toLowerCase())
        if (!categoryId) {
          rowErrors.push(`Category "${row.category}" not found. Available categories: ${categories.map(c => c.name).join(', ')}`)
        }
      }

      if (!row.price || isNaN(parseFloat(row.price))) {
        rowErrors.push('Valid price is required')
      }

      if (rowErrors.length > 0) {
        errors.push({
          row: rowNumber,
          data: row,
          errors: rowErrors
        })
      } else {
        // Parse and add valid item
        const categoryId = categoryMap.get(row.category.toLowerCase())

        valid.push({
          name: row.name.trim(),
          description: row.description?.trim() || '',
          price: parseFloat(row.price),
          categoryId: categoryId,
          allergens: row.allergens ? row.allergens.split(';').map((a: string) => a.trim()).filter((a: string) => a) : [],
          dietary: row.dietary ? row.dietary.split(';').map((d: string) => d.trim()).filter((d: string) => d) : [],
          available: row.available?.toLowerCase() !== 'false' && row.available?.toLowerCase() !== 'no',
          featured: row.featured?.toLowerCase() === 'true' || row.featured?.toLowerCase() === 'yes',
          image: null,
          imageUrl: row.imageUrl?.trim() || ''
        })
      }
    })

    return { valid, errors }
  }

  // Generate CSV template
  const generateTemplate = (): string => {
    const headers = [
      'name',
      'category',
      'description',
      'price',
      'allergens',
      'dietary',
      'available',
      'featured',
      'imageUrl'
    ]

    const exampleRows = [
      [
        'Margherita Pizza',
        'Main Courses',
        'Classic tomato sauce, fresh mozzarella, and basil',
        '89.99',
        'Dairy;Wheat',
        'Vegetarian',
        'true',
        'false',
        ''
      ],
      [
        'Caesar Salad',
        'Appetizers',
        'Romaine lettuce, croutons, parmesan, caesar dressing',
        '65.00',
        'Dairy;Eggs',
        'Vegetarian',
        'true',
        'true',
        ''
      ],
      [
        'Chocolate Brownie',
        'Desserts',
        'Rich chocolate brownie with vanilla ice cream',
        '45.00',
        'Dairy;Eggs;Wheat;Nuts',
        'Vegetarian',
        'true',
        'false',
        ''
      ]
    ]

    const csvContent = [
      headers.join(','),
      ...exampleRows.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n')

    return csvContent
  }

  // Download CSV template
  const downloadTemplate = () => {
    const csv = generateTemplate()
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)

    link.setAttribute('href', url)
    link.setAttribute('download', 'menu-items-template.csv')
    link.style.visibility = 'hidden'

    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }

  // Read file as text
  const readFileAsText = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()

      reader.onload = (e) => {
        if (e.target?.result) {
          resolve(e.target.result as string)
        } else {
          reject(new Error('Failed to read file'))
        }
      }

      reader.onerror = () => {
        reject(new Error('Error reading file'))
      }

      reader.readAsText(file)
    })
  }

  // Import menu items from CSV file
  const importFromCSV = async (file: File, categories: any[]): Promise<{ valid: any[], errors: any[] }> => {
    try {
      // Validate file type
      if (!file.name.endsWith('.csv')) {
        throw new Error('Please upload a CSV file')
      }

      // Read file
      const csvText = await readFileAsText(file)

      // Parse CSV
      const data = parseCSV(csvText)

      if (data.length === 0) {
        throw new Error('No data found in CSV file')
      }

      // Validate and transform data
      const result = validateMenuItems(data, categories)

      return result
    } catch (error) {
      throw error
    }
  }

  return {
    parseCSV,
    validateMenuItems,
    generateTemplate,
    downloadTemplate,
    importFromCSV
  }
}
