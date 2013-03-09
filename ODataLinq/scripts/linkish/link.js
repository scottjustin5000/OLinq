var Query = function (col) {

    var collection = col,
        where = function(expression) {
            var modified = [];
            var len = collection.length;

            for (var i = 0; i < len; i++) {
                if (expression.apply(collection[i], [collection[i], i])) {
                    modified[modified.length] = collection[i];
                }
            }
            collection = modified;
            return this;
        },
          first = function (expression) {
              if (expression !== undefined) {
                  where(expression);

                  return first();
              }
              else {
                  if (collection.length > 0) {
                      return collection[0];
                  } else {
                      return null;
                  }
              }
          },
    last = function (expression) {
        if (expression !== undefined) {
            where(expression);
            return last();
        }
        else {
            if (collection.length > 0) {
                return collection[collection.length - 1];
            } else {
                return null;
            }
        }
    },
    firstOrDefault= function (value) {
            return first() || value;
        },
    lastOrDefault= function (value) {
        return last() || value;
    },
    elementAt = function (i) {
        return collection[i];
    },
     orderBy = function (expression) {
           
            var tempArray = [];
            for (var i = 0; i < collection.length; i++) {
                tempArray[tempArray.length] =collection[i];
            }

            if (typeof(expression) !== "function") {
                var field = expression;

                expression = function() { return this[field]; };

            }
            collection =
                tempArray.sort(function(a, b) {
                    var x = expression.apply(a, [a]);
                    var y = expression.apply(b, [b]);
                    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
                });
            return this;
        },
        orderByDescending = function(expression) {
            var tempArray = [], field;
            for (var i = 0; i < collection.length; i++) {
                tempArray[tempArray.length] = collection[i];
            }

            if (typeof(expression) !== "function") {
                field = expression;
                expression = function() { return this[field]; };
            }

            collection = tempArray.sort(function(a, b) {
                var x = expression.apply(b, [b]), y = expression.apply(a, [a]);
                return ((x < y) ? -1 : ((x > y) ? 1 : 0));
            });
            return this;
        },
        merge = function(coll) {
            var newcoll = coll.items || coll;
            collection.concat(newcoll);
            return this;
        },
        select = function (expression) {
            var item;
            var newArray = [];
            var field = expression;
            if (typeof (expression) !== "function") {
                if (expression.indexOf(",") === -1) {
                    expression = function () { return this[field]; };
                } else {
                    expression = function () {
                     
                        var fields = field.split(",");
                        var obj = {};
                        for (var i = 0; i < fields.length; i++) {
                            obj[fields[i]] = this[fields[i]];
                        }
                        return obj;
                    };
                }
            }
            for (var i = 0; i < collection.length; i++) {
                item = expression.apply(collection[i], [collection[i]]);
                if (item) {
                    newArray[newArray.length] = item;
                }
            }
            collection = newArray;
            return this;
        },
        selectMany = function(expression) {
            var results = [];
            for (var i = 0; i < collection.length; i++) {
                results = results.concat(expression.apply(collection[i], [collection[i]]));
            }
            collection = results;
            return this;
        },
        count= function (expression) {
                if (expression === undefined) {
                    return collection.length;
                } else {
                    where(expression);
                    return collection.length;
                }
         },
    distinct= function (expression) {
        var item;
        var dict = {};
        var retVal = [];
        for (var i = 0; i < collection.length; i++) {
            item = expression.apply(collection[i], [collection[i]]);
            if (!assertObjectEqual(dict[item], item)) {
                
                dict[item] = item;
                retVal.push(item);
            }
         
        }
        collection = retVal;
        return this;
    },
    any= function (expression) {
        for (var i = 0; i < collection.length; i++) {
            if (expression.apply(collection[i], [collection[i], i])) { return true; }
        }
        return false;
    },
    reverse= function () {
            var modified = [];
            for (var i = collection.length - 1; i > -1; i--) {
                modified[modified.length] = collection[i];
            }
            collection = modified;
            return this;
    },
   take = function (cnt) {
       where(function (item, index) { return index < cnt; });
       return this;
   },
    skip = function (cnt) {
        where(function (item, index) { return index >= cnt; });
        return this;
    },
  
    execute = function() {
            return collection;
        };

    return {
        where: where,
        orderBy: orderBy,
        orderByDescending: orderByDescending,
        select: select,
        selectmany:selectMany,
        any: any,
        count: count,
        first: first,
        last: last,
        firstOrDefault: firstOrDefault,
        lastOrDefault:lastOrDefault,
        reverse: reverse,
        take: take,
        skip:skip,
        elementAt:elementAt,
        distinct:distinct,
        execute: execute
    };

};
