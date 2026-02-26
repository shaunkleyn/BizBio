// Checkered Block - Ecwid Initialization Script
(function() {
  'use strict';
  
  // Load SEO enhancements
  var seoScript = document.createElement('script');
  seoScript.src = 'https://bizbio.co.za/checkeredblock/ecwid/ecwid-seo-enhancements.js';
  seoScript.defer = true;
  document.head.appendChild(seoScript);
  
  // SociableKit Reviews Integration
  function addElement(target, newClass, embedId, script) {
    if ($('.' + newClass).length === 0) {
      $('<div>')
        .attr('data-embed-id', embedId)
        .addClass(newClass)
        .insertAfter(target);
      console.log('Div with class ' + newClass + ' added.');
      
      var newScript = document.createElement('script');
      newScript.src = script;
      newScript.onload = function() {
        console.log('Confirmation: ' + script + ' has loaded!');
      };
      document.body.appendChild(newScript);
    } else {
      console.log('Div with class ' + newClass + ' already exists, skipping addition.');
    }
  }
  
  function executeAfterFullPageLoad() {
    console.log('my-script.js: All page resources are loaded!');
    
    if ($('.sk-ww-google-reviews').length === 0) {
      $('<div>')
        .attr('data-embed-id', '25569540')
        .addClass('sk-ww-google-reviews')
        .insertAfter('.ins-tile--customer-review .ins-tile__title');
      console.log('Div with class "sk-ww-google-reviews" added.');
    } else {
      console.log('Div with class "sk-ww-google-reviews" already exists, skipping addition.');
    }
    
    const scriptUrl = 'https://widgets.sociablekit.com/google-reviews/widget.js';
    if ($('script[src="' + scriptUrl + '"]').length === 0) {
      const newScript = document.createElement('script');
      newScript.src = scriptUrl;
      newScript.async = true;
      $('body').append(newScript);
    }
  }
  
  // Remove dark sidebar class from header
	document.addEventListener('DOMContentLoaded', function() {
	  var header = document.querySelector('.ins-tile--header.ins-tile--dark-sidebar');
	  if (header) {
		header.classList.remove('ins-tile--adapted-background');
		console.log('Dark header classes removed');
	  }
	});
  
  window.addEventListener('load', function() {
    console.log('Window.onload fired: All page assets (including images) are now loaded!');
    executeAfterFullPageLoad();
  });
  
})();