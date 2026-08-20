$(document).ready(function () {

    $("#mytable").DataTable({
        ajax: {
            url: "/Product/GetData",
            type: "GET",
            dataSrc: "data"
        },
        columns: [
            {
                data: "img",
                orderable: false,
                searchable: false,
                render: function (img) {
                    var src = img ? '/' + img : '/img/default-150x150.png';
                    return '<img src="' + src + '" style="width:60px;height:60px;object-fit:cover;border-radius:6px;" />';
                }
            },
            { data: "name" },
            { data: "description" },
            { data: "price" },
            { data: "categoryName" },
            {
                data: "id",
                render: function (id) {
                    return `
                        <a href="/Product/Edit/${id}" class="btn btn-success btn-sm">
                            <i class="fa-solid fa-pen"></i>
                        </a>

                        <button class="btn btn-danger btn-sm">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    `;
                }
            }
        ],
        autoWidth: false,
        scrollX: true
    });

});