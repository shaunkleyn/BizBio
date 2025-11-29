  $(function() {
    // email protection
    $('.protected-link').each(function(){
      var mAddress = $(this).html().replaceAll('--at--','@').replaceAll('--dot--','.');
      $(this).html(mAddress);
    });
    


  });

$(document).ready(function(){
  replaceProtectedLinks('body');
});

function replaceProtectedLinks(element) {
  const replacementMap = {
    '--dot--': '.',
    '--at--': '@',
    '--dash--': '-',
    '--slash--': '/',
    '--plus--': '+',
    '--two--': '2',
    '--seven--': '7',

    };

    $(element).find('.protected-link').each(function(){
      try {
    var el = $(this);
    $.each(replacementMap, function(i,e) {
        var mAddress = $(el).html().replaceAll(i,e);
      $(el).html(mAddress);
        var mAddress = $(el).attr('href').replaceAll(i,e);
      $(el).attr('href', mAddress);
    })
      } catch (error) {
        console.error("Error processing element:", el, error);
      }
    });
  }