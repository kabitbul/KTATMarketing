$(document).ready( function () {
  $.fn.dataTable.ext.search.push(
    function( settings, searchData, index, rowData, counter ) {
      var positions = $('input:checkbox[name="pos"]:checked').map(function() {
        return this.value;
      }).get();
      if (positions.length === 0) {
        return true;
      }
      if (positions.indexOf(searchData[1]) !== -1) {
        return true;
      }   
      return false;
    }
  )
    var table = $('#tblData').DataTable();
 $('input:checkbox').on('change', function () {
    table.draw();
 });
} );