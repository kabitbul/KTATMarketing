﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Warehouse/Product/GetAll"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "productName", "width": "20%" },
            { "data": "cost" , "width": "10%" },
            { "data": "inventoryCount", "width": "10%" },
            { "data": "onTheWayInventory", "width": "10%" },
            {
                "data": "ownByWarehouse",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked/>`
                    }
                    else {
                        return `<input type="checkbox" disabled/>`
                    }
                }, "width": "10%"
            },
            {
                "data": "reStock",
                "render": function (data)
                {
                    if (data) {
                        return `<input type="checkbox" disabled checked/>`
                    }
                    else {
                        return `<input type="checkbox" disabled />`
                    }
                         },"width": "10%"
            },
            { "data": "category.name", "width": "10%" },
            { "data": "weight", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Warehouse/Product/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                         `;
                }, "width": "5%"
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