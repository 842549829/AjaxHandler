/*-----------------------------------------------------------------
功能说明：   %H_DES%
创建时间：   %H_DATE%
其他信息：   此文件依赖 json2.js <http://www.json.org/json2.js> 和 jquery.js 
------------------------------------------------------------------*/
(function($){
    if(!$.net){
        var defaultOptions = {
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            type:"POST"
            //, complete: function(r, status,sss) { debugger; } // 此代码加上用于全局调试 
        };

        // 将net作为命名空间扩展到jQuery框架内
        $.extend({ net: {} });

        // 将调用Web服务的代理函数CallWebMethod扩展到jQuery框架内
        $.extend($.net,{
            CallWebMethod: function(options, method, args, successCallback, errorCallback){
                
                // 调用第三方对象序列化成 json字符串方法
                var jsonStr = JSON.stringify(args);
                var parameters = $.extend({}, defaultOptions);
                var url0 = options.url + "/" + method;
                $.extend(parameters, options, { url: url0, data: jsonStr, success: successCallback, error: errorCallback });
                $.ajax(parameters);
            }
        });
    }

    // 将指定类型的Web服务扩展到jQuery框架内
    var services = new %CLS%();
    $.extend($.net, { %CLS%: services });
})(jQuery);

// 功能说明：服务的构造函数
function %CLS%()
{
    // 定义本地的调用选项，如果希望改变个别的ajax调用选项，请在对象中添加其他选项的键/值
    this.Options = { url: "%URL%" };
}