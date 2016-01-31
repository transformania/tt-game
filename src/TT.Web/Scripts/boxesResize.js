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

boxes3 = $('.rp-ad');
maxHeight3 = Math.max.apply(
Math, boxes3.map(function () {
    return $(this).height();
}).get());
boxes3.height(maxHeight3);