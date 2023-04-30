﻿$(function () {

    

    $(document).on("click", ".slider-status", function () {

        let sliderId = $(this).parent().attr("data-id")

        let changeElem = $(this)

        let data = { id: sliderId }


        $.ajax({
            url: "slider/setstatus",
            type: "Post",
            data: data,
            success: function (res) {

                if (res) {

                    $(changeElem).removeClass("active-status")
                    $(changeElem).addClass("deActive-status")

                }
                else{

                    $(changeElem).removeClass("deActive-status")
                    $(changeElem).addClass("active-status")

                }

            }
        })

    })






    $(document).on("submit", "#category-delete-form", function (e) {

        e.preventDefault();

        let categoryId = $(this).attr("data-id")

        let deletedElem = $(this)

        let data = { id: categoryId }


        $.ajax({
            url: "category/softdelete",
            type: "Post",
            data: data,
            success: function (res) {

               $(deletedElem).parent().parent().remove()

            }
        })

    })
})