$(function () {
  //$(".datum").datepicker({
  //    // dateFormat: "dd.mm.yy"            
  //});

  $("[data-autocomplete]").each(function (index, element) {
    var url = $(element).data('autocomplete');
    var resultplaceholder = $(element).data('autocomplete-placeholder-name');
    if (resultplaceholder === undefined)
      resultplaceholder = url;

    $(element).change(function () {
      var dest = $("[data-autocomplete-placeholder='" + resultplaceholder + "']");
      var text = $(element).val();
      if (text.length === 0 || text !== $(dest).data('selected-label')) {
        $(dest).val('');
      }
    });

    $(element).autocomplete({
      source: "/autocomplete/" + url,
      autoFocus: true,
      minLength: 1,
      select: function (event, ui) {
        $(element).val(ui.item.label);
        var dest = $("[data-autocomplete-placeholder='" + resultplaceholder + "']");
        $(dest).val(ui.item.id);
        $(dest).data('selected-label', ui.item.label);

        //za artikle...
        var dest_cijena = $("[data-autocomplete-placeholder-cijena='" + resultplaceholder + "']");
        if (dest_cijena !== undefined) {
          $(dest_cijena).val(ui.item.cijena);
        }
      }
    });
  });
});