  $(function() {
    // email protection
    $('.bb-eaddress, .protected-link').each(function(){
      var mAddress = $(this).html().replaceAll('--at--','@').replaceAll('--dot--','.');
      $(this).html(mAddress).attr('href', 'mailto:' + mAddress);
    });
    

    const replacementMap = {
    '--dot--': '.',
    '--at--': '@',
    '--dash--': '-',
    '--slash--': '/',
    '--plus--': '+',
    '--two--': '2',
    '--seven--': '7',

    };

    // Create a regex pattern from all aliases
    const aliasPattern = new RegExp(Object.keys(replacementMap).join('|'), 'g');

    function replaceAliases(text) {
        return text.replace(aliasPattern, (match) => replacementMap[match]);
    }

    document.addEventListener('DOMContentLoaded', function() {
        document.querySelectorAll('a, span, div, p').forEach(element => {
            element.innerHTML = replaceAliases(element.innerHTML);
        });
    });
  });