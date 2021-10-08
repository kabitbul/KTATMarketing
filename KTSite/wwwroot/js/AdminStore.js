var dataTable;

$(document).ready(function () { /*test*/
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/AdminStore/GetAllAdmin"
        },
        "columns": [
            { "data": "storeName", "width": "80%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a onclick=Delete("/Admin/AdminStore/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"
                              data-toggle="tooltip" data-placement="top" title="Delete Store">
                                <i class="fas fa-trash-alt"></i>&nbsp;
                            </a>
                        </div>
                         `;
                }, "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}