boxes = $('.charaBox');
maxHeight = Math.max.apply(
Math, boxes.map(function () {
    return $(this).height();
}).get());
boxes.height(maxHeight);

boxes2 = $('.itemBox');
maxHeight2 = Math.max.apply(
Math, boxes2.map(function () {
    return $(this).height();
}).get());
boxes2.height(maxHeight2);