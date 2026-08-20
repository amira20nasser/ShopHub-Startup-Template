$(document).ready(function () {

    var currentPage = 1;
    var pageSize = 5;
    var searchTerm = "";
    var sortBy = "name";
    var sortOrder = "asc";

    function loadProducts() {
        $.ajax({
            url: "/Product/GetData",
            type: "GET",
            data: {
                pageNumber: currentPage,
                pageSize: pageSize,
                searchTerm: searchTerm,
                sortBy: sortBy,
                sortOrder: sortOrder
            },
            success: function (result) {
                renderTable(result.items);
                renderPagination(result);
                renderPageInfo(result);
            }
        });
    }

    function renderTable(items) {
        var tbody = $("#productBody");
        var emptyState = $("#emptyState");
        tbody.empty();

        if (!items || items.length === 0) {
            $("#productTable").hide();
            emptyState.show();
            return;
        }

        $("#productTable").show();
        emptyState.hide();

        $.each(items, function (i, product) {
            var src = product.img ? '/' + product.img : '/img/default-150x150.png';
            var row = '<tr>' +
                '<td><img src="' + src + '" style="width:60px;height:60px;object-fit:cover;border-radius:6px;" /></td>' +
                '<td>' + (product.name || '') + '</td>' +
                '<td>' + (product.description || '') + '</td>' +
                '<td>$' + (product.price || 0).toFixed(2) + '</td>' +
                '<td>' + (product.categoryName || '') + '</td>' +
                '<td>' +
                    '<a href="/Product/Edit/' + product.id + '" class="btn btn-success btn-sm"><i class="fa-solid fa-pen"></i></a> ' +
                    '<button class="btn btn-danger btn-sm delete-btn" data-id="' + product.id + '"><i class="fa-solid fa-trash"></i></button>' +
                '</td>' +
                '</tr>';
            tbody.append(row);
        });
    }

    function renderPagination(result) {
        var pagination = $("#pagination");
        pagination.empty();

        if (result.totalPages <= 1) return;

        pagination.append(
            '<button ' + (!result.hasPrevious ? 'disabled' : '') + ' data-page="' + (result.pageNumber - 1) + '">&#8249; Prev</button>'
        );

        for (var p = 1; p <= result.totalPages; p++) {
            pagination.append(
                '<button class="' + (p === result.pageNumber ? 'active' : '') + '" data-page="' + p + '">' + p + '</button>'
            );
        }

        pagination.append(
            '<button ' + (!result.hasNext ? 'disabled' : '') + ' data-page="' + (result.pageNumber + 1) + '">Next &#8250;</button>'
        );
    }

    function renderPageInfo(result) {
        var start = result.totalItems === 0 ? 0 : (result.pageNumber - 1) * result.pageSize + 1;
        var end = Math.min(result.pageNumber * result.pageSize, result.totalItems);
        $("#pageInfo").text("Showing " + start + "-" + end + " of " + result.totalItems + " products");
    }

    $(document).on("click", ".pagination button", function () {
        var page = $(this).data("page");
        if (page && !$(this).is(":disabled")) {
            currentPage = page;
            loadProducts();
        }
    });

    var searchTimer;
    $("#searchInput").on("keyup", function () {
        clearTimeout(searchTimer);
        var val = $(this).val();
        searchTimer = setTimeout(function () {
            searchTerm = val;
            currentPage = 1;
            loadProducts();
        }, 400);
    });

    $("#sortSelect").on("change", function () {
        var val = $(this).val();
        var parts = val.split("_");
        sortBy = parts[0];
        sortOrder = parts[1];
        currentPage = 1;
        loadProducts();
    });

    $(document).on("click", ".delete-btn", function () {
        var id = $(this).data("id");
        if (confirm("Are you sure you want to delete this product?")) {
            $.ajax({
                url: "/Product/Delete/" + id,
                type: "DELETE",
                success: function (result) {
                    if (result.success) {
                        loadProducts();
                    } else {
                        alert(result.message);
                    }
                }
            });
        }
    });

    loadProducts();

});
