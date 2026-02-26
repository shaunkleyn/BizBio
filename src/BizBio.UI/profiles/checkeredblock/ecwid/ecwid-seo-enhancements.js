// Checkered Block Shop - SEO Enhancements for Ecwid
// Version 1.0

(function() {
  'use strict';
  
  // Configuration
  const config = {
    siteName: 'Checkered Block Shop',
    siteUrl: 'https://shop.checkeredblock.co.za',
    currency: 'ZAR',
    logo: 'https://d2gt4h1eeousrn.cloudfront.net/72052286/header-fcHJMd/kPM3YD3-200x200.png'
  };

  // Wait for Ecwid to load
  window.ecwid_script_defer = false;
  window.ecwid_dynamic_widgets = false;

  function initSEOEnhancements() {
    if (typeof Ecwid === 'undefined') {
      console.log('Ecwid not loaded yet, waiting...');
      setTimeout(initSEOEnhancements, 500);
      return;
    }

    Ecwid.OnAPILoaded.add(function() {
      console.log('Checkered Block SEO enhancements loaded');
      
      // Initialize all enhancements
      initProductSchema();
      initBreadcrumbs();
      initMetaUpdates();
      initCanonicalUrls();
      initFAQSchema();
      initImageAltText();
      initPerformanceOptimizations();
    });
  }

  // 1. Product Structured Data
  function initProductSchema() {
    Ecwid.OnPageLoaded.add(function(page) {
      if (page.type !== 'PRODUCT') return;
      
      Ecwid.getProduct(page.productId, function(product) {
        removeExistingSchema('product-schema');
        
        const productSchema = {
          "@context": "https://schema.org",
          "@type": "Product",
          "name": product.name,
          "description": stripHtml(product.description).substring(0, 200),
          "image": product.imageUrl || product.thumbnailUrl,
          "sku": product.sku || product.id.toString(),
          "offers": {
            "@type": "Offer",
            "url": window.location.href,
            "priceCurrency": config.currency,
            "price": product.price || product.defaultDisplayedPrice,
            "availability": product.inStock ? 
              "https://schema.org/InStock" : 
              "https://schema.org/OutOfStock",
            "seller": {
              "@type": "Organization",
              "name": config.siteName
            }
          }
        };
        
        if (product.brand) {
          productSchema.brand = {
            "@type": "Brand",
            "name": product.brand
          };
        }
        
        if (product.rating && product.reviewsCount > 0) {
          productSchema.aggregateRating = {
            "@type": "AggregateRating",
            "ratingValue": product.rating,
            "reviewCount": product.reviewsCount
          };
        }
        
        addSchemaToHead(productSchema, 'product-schema');
      });
    });
  }

  // 2. Breadcrumb Structured Data
  function initBreadcrumbs() {
    Ecwid.OnPageLoaded.add(function(page) {
      if (page.type !== 'PRODUCT' && page.type !== 'CATEGORY') return;
      
      const breadcrumbSchema = {
        "@context": "https://schema.org",
        "@type": "BreadcrumbList",
        "itemListElement": [
          {
            "@type": "ListItem",
            "position": 1,
            "name": "Home",
            "item": config.siteUrl + "/"
          }
        ]
      };
      
      if (page.categoryId) {
        Ecwid.getCategory(page.categoryId, function(category) {
          breadcrumbSchema.itemListElement.push({
            "@type": "ListItem",
            "position": 2,
            "name": category.name,
            "item": config.siteUrl + "/#!category/" + category.id
          });
          
          if (page.type === 'PRODUCT') {
            Ecwid.getProduct(page.productId, function(product) {
              breadcrumbSchema.itemListElement.push({
                "@type": "ListItem",
                "position": 3,
                "name": product.name,
                "item": window.location.href
              });
              
              removeExistingSchema('breadcrumb-schema');
              addSchemaToHead(breadcrumbSchema, 'breadcrumb-schema');
            });
          } else {
            removeExistingSchema('breadcrumb-schema');
            addSchemaToHead(breadcrumbSchema, 'breadcrumb-schema');
          }
        });
      }
    });
  }

  // 3. Dynamic Meta Updates
  function initMetaUpdates() {
    Ecwid.OnPageLoaded.add(function(page) {
      if (page.type === 'PRODUCT') {
        Ecwid.getProduct(page.productId, function(product) {
          const productDesc = stripHtml(product.description).substring(0, 155);
          const newDesc = product.name + " - " + productDesc + " | " + config.siteName;
          
          updateMetaTag('description', newDesc);
          updateOGTag('og:title', product.name + ' | ' + config.siteName);
          updateOGTag('og:description', productDesc);
          updateOGTag('og:image', product.imageUrl || product.thumbnailUrl);
          updateOGTag('og:url', window.location.href);
          updateOGTag('og:type', 'product');
          updateOGTag('product:price:amount', product.price);
          updateOGTag('product:price:currency', config.currency);
        });
      }
      
      if (page.type === 'CATEGORY') {
        Ecwid.getCategory(page.categoryId, function(category) {
          const categoryDesc = category.description ? 
            stripHtml(category.description).substring(0, 140) : 
            'Browse our ' + category.name.toLowerCase() + ' collection';
          const newDesc = category.name + " - " + categoryDesc + " | " + config.siteName;
          
          updateMetaTag('description', newDesc);
          updateOGTag('og:title', category.name + ' | ' + config.siteName);
          updateOGTag('og:description', categoryDesc);
        });
      }
    });
  }

  // 4. Canonical URLs
  function initCanonicalUrls() {
    Ecwid.OnPageLoaded.add(function(page) {
      const canonical = document.querySelector('link[rel="canonical"]');
      const currentUrl = window.location.href.split('?')[0];
      
      if (canonical) {
        canonical.setAttribute('href', currentUrl);
      } else {
        const link = document.createElement('link');
        link.rel = 'canonical';
        link.href = currentUrl;
        document.head.appendChild(link);
      }
    });
  }

  // 5. FAQ Structured Data
  function initFAQSchema() {
    window.addEventListener('load', function() {
      const faqSchema = {
        "@context": "https://schema.org",
        "@type": "FAQPage",
        "mainEntity": [
          {
            "@type": "Question",
            "name": "What payment methods do you accept?",
            "acceptedAnswer": {
              "@type": "Answer",
              "text": "We accept secure payments via PayFast including credit cards, instant EFT, and Mobicred for 12-month payment plans."
            }
          },
          {
            "@type": "Question",
            "name": "How long does delivery take?",
            "acceptedAnswer": {
              "@type": "Answer",
              "text": "Delivery times vary depending on your location and product availability. We use The Courier Guy and PUDO Locker for nationwide deliveries across South Africa."
            }
          },
          {
            "@type": "Question",
            "name": "What is your return policy?",
            "acceptedAnswer": {
              "@type": "Answer",
              "text": "We have a 5-day return policy. Items must be in the same condition you received them, unworn or unused, with tags, and in original packaging. Proof of purchase is required."
            }
          },
          {
            "@type": "Question",
            "name": "Do you offer custom printing services?",
            "acceptedAnswer": {
              "@type": "Answer",
              "text": "Yes! We specialize in custom printing for t-shirts, promotional products, digital business cards, stamps, and professional branding materials. Contact us for custom orders."
            }
          }
        ]
      };
      
      addSchemaToHead(faqSchema, 'faq-schema');
    });
  }

  // 6. Add Alt Text to Images
  function initImageAltText() {
    window.addEventListener('load', function() {
      setTimeout(function() {
        const images = document.querySelectorAll('img:not([alt]), img[alt=""]');
        
        images.forEach(function(img) {
          let altText = '';
          
          // Try to get text from nearby elements
          const parent = img.closest('.ec-store__category-item, .grid-product__content, .ec-grid-product');
          if (parent) {
            const productName = parent.querySelector('.grid-product__title, .ec-grid-product__title');
            if (productName) {
              altText = productName.textContent.trim();
            }
          }
          
          // Fallback to filename
          if (!altText && img.src) {
            const filename = img.src.split('/').pop().split('?')[0];
            altText = filename.replace(/\.(jpg|jpeg|png|gif|webp)$/i, '').replace(/[-_]/g, ' ');
          }
          
          if (altText) {
            img.setAttribute('alt', altText + ' - ' + config.siteName);
          } else {
            img.setAttribute('alt', config.siteName + ' product image');
          }
        });
      }, 2000);
    });
  }

  // 7. Performance Optimizations
  function initPerformanceOptimizations() {
    // Preconnect to CDN domains
    const domains = [
      'https://d2gt4h1eeousrn.cloudfront.net',
      'https://d2j6dbq0eux0bg.cloudfront.net'
    ];
    
    domains.forEach(function(domain) {
      const link = document.createElement('link');
      link.rel = 'preconnect';
      link.href = domain;
      link.crossOrigin = 'anonymous';
      document.head.appendChild(link);
    });
  }

  // Utility Functions
  function stripHtml(html) {
    const tmp = document.createElement('DIV');
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || '';
  }

  function addSchemaToHead(schema, dataAttribute) {
    const script = document.createElement('script');
    script.type = 'application/ld+json';
    script.setAttribute('data-schema-type', dataAttribute);
    script.textContent = JSON.stringify(schema);
    document.head.appendChild(script);
  }

  function removeExistingSchema(dataAttribute) {
    const existing = document.querySelector('script[data-schema-type="' + dataAttribute + '"]');
    if (existing) {
      existing.remove();
    }
  }

  function updateMetaTag(name, content) {
    let meta = document.querySelector('meta[name="' + name + '"]');
    if (meta) {
      meta.setAttribute('content', content);
    } else {
      meta = document.createElement('meta');
      meta.name = name;
      meta.content = content;
      document.head.appendChild(meta);
    }
  }

  function updateOGTag(property, content) {
    let tag = document.querySelector('meta[property="' + property + '"]');
    if (tag) {
      tag.setAttribute('content', content);
    } else {
      tag = document.createElement('meta');
      tag.setAttribute('property', property);
      tag.content = content;
      document.head.appendChild(tag);
    }
  }

  // Initialize everything
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSEOEnhancements);
  } else {
    initSEOEnhancements();
  }

})();