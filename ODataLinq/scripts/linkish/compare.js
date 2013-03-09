
function getKeys(obj) {

    var keys;

        if (obj.keys) {
            keys = obj.keys();
        } else {
            keys = [];

            for (var k in obj) {
                if (Object.prototype.hasOwnProperty.call(obj, k)) {
                    keys.push(k);
                }
            }
        } 
    return keys;
}
function reconstructObject(obj, keys) {
    var result = {};
    for (var i = 0, l = keys.length; i < l; i++) {
        if (Object.prototype.hasOwnProperty.call(obj, keys[i])) {
            result[keys[i]] = obj[keys[i]];
        }
    }

    return result;
}

function assertObjectEqual(a, b) {

    if (a === undefined && b !== undefined || b === undefined && a !== undefined) {
        
        return false;
    }
    if (Object.prototype.toString.call(a) === '[object Array]' && Object.prototype.toString.call(b) === '[object Array]') {
     
        if (a.filter(function (e) { return Object.prototype.toString.call(e) === '[object Object]' }).length > 0 ||
            b.filter(function (e) { return Object.prototype.toString.call(e) === '[object Object]' }).length > 0) {

            if (a.length !== b.length) {
              return  JSON.stringify(a)===JSON.stringify(b);
            } else {
                for (var i = 0, l = a.length; i < l; i++) {
                    assertObjectEqual(a[i], b[i]);
                }
            }
        } else {
            return JSON.stringify(a)=== JSON.stringify(b);
        }
    } else {

        var orderedA = reconstructObject(a, getKeys(a).sort()),
            orderedB = reconstructObject(b, getKeys(b).sort());

        return JSON.stringify(orderedA)=== JSON.stringify(orderedB);
    }

   /* if (a === undefined && b !== undefined || b === undefined && a !== undefined) {
        return false;
    }
    if (Object.prototype.toString.call(a) === '[object Array]' && Object.prototype.toString.call(b) === '[object Array]') {
        return JSON.stringify(a) === JSON.stringify(b);
    } else {
        
        var orderedA = reconstructObject(a, getKeys(a).sort()),
            orderedB = reconstructObject(b, getKeys(b).sort());
        console.log(getKeys(a).sort());
        console.log(orderedB);
        // compare as strings for diff tolls to show us the difference
        return JSON.stringify(orderedA)=== JSON.stringify(orderedB);
    }*/
}