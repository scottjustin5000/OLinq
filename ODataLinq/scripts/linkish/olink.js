var Olink = function (service) {
    var
        url = service,
        collection,
        whereClause,
        skipClause,
        takeClause,
        orderClause,
        orderDirection,
        andClauses,
        isCount = false,
        request,
        fromCollection = function(col) {
            collection = col;
            return this;
        },
        where = function(expression) {
            whereClause = expression;
            return this;
        },
        and = function (expression) {
            if (!andClauses) {
                andClauses = new Array();
            }
            andClauses.push(expression);
            return this;
        };
    skip = function(count) {
        skipClause = count;
        return this;
    },
    take = function(count) {
        takeClause = count;
        return this;
    },
    orderBy = function(expression, dir) {
        orderDirection = dir;
        orderClause = expression;
    },
    select = function(expression) {

    },
    execute = function() {
        getCompleteFilter();
        
        if (!isCount) {
            request += '&$format=json';
        }

        return request;
    },
    getCompleteFilter = function () {
        request = url + '/' + collection + '?' + '$filter=';
    
       request += getWhere(whereClause);
       
        if (andClauses) {
           for (var i = 0; i < andClauses.length; i++) {
                  request += ' and ' + getWhere(andClauses[i]);
             }
        }

    },
    getWhere = function (express) {
       
        var exp = new Expression(express);
        if (typeof (whereClause) !== "function") {
            return exp.parse();
        }
        return exp.parseFunction();

    };
    /*
      var query = new oQuery("http://odata.netflix.com/Catalog") 
            .From("Titles") 
            .Where("item.DateModified >= date") 
            .Where("item.Name.Contains('blue')") 
            .Take(100); 
    */
    return {
        from: fromCollection,
        where: where,
        and: and,
        skip: skip,
        take: take,
        orderBy: orderBy,
        execute:execute
    };

};