angular.module('xxx.cinemaHq.app.bookings')
    .controller('BookingController', [
        '$scope', '$modal', 'baseGridEditorFactory', 'bookingGridFactory', 'bookingEditorFactory', 'errorAlerterFactory',
        function ($scope, $modal, baseGridEditorFactory, bookingGridFactory, bookingEditorFactory, errorAlerterFactory) {
            'use strict';

            var gridEditorScope = $scope.$new(true);

            var grid = bookingGridFactory({
                $scope: gridEditorScope
            });

            var editor = bookingEditorFactory({
                $scope: gridEditorScope
            });

            var gridEditor = baseGridEditorFactory({
                grid: grid,
                editor: editor,
                $scope: gridEditorScope
            });

            angular.extend($scope, {
                gridEditor: gridEditor,
                openAddBookingsModal: openAddBookingsModal,
                errorAlerter: errorAlerterFactory()
            });
            
            gridEditor.refresh();

            gridEditorScope.$on('error', function (event, error) {
                $scope.errorAlerter.add(error);
            });

            function openAddBookingsModal() {
                var modalInstance = $modal.open({
                    templateUrl: 'js/bookings/addBookingsModal.html',
                    size: 'lg',
                    controller: 'addBookingsModalController',
                });

                modalInstance.result.then(function() {
                    gridEditor.refresh();
                });
            }
        }
    ]);