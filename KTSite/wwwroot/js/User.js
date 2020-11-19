var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "role", "width": "20%" },
              {
                "data":
                {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    var role = data.role;
                    if (role == "Admin")
                    {
                        return `
                             <div class="text-center">
                                  `
                    }
                    else if (lockout > today) {
                        return `
                             <div class="text-center">
                               <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px">
                               <i class="fas fa-lock-open"></i> Unlock
                                  `
                    }
                    else {
                        return `
                             <div class="text-center">
                               <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer"; width:100px">
                               <i class="fas fa-lock"></i> Lock
                                  `
                    }

                  }, "width": "20%"
            }
        ]
    });
}

function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
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