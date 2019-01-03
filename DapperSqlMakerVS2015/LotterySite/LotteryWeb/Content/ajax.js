var xhr = createXHR();
//1 获取对象
function createXHR() {
    var request;
    if (typeof (XMLHttpRequest) === 'undefined') {
        request = new ActiveObject('microsoft.XMLTTP');
    }
    else {
        request = new XMLHttpRequest();
    }
    return request;
}
function ajax(method, url, isAsync, postData, funsuccess, funerror) {
    //请求方式，请求页面路径，是否异步，post请求参数，回调方法
    //同步处理/异步处理    get只能发送异步请求  
    //2 初始化
    xhr.open(method, url, isAsync);
    if (method === 'post') {
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    }
    //3 注册事件
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) { //接收完数据的状态
            if (xhr.status === 200) { //响应码为正常
                var r = xhr.responseText; //将输出值传给方法；
                funsuccess(r);  //注意参数的使用 回调函数
            } else { //异常
                funerror();
            }
        }
    }
    //4 发送请求
    xhr.send(postData);
}
//get请求
function get(url,funsuccess) {
    ajax('get', url, true, null, funsuccess, function () { });
}
//post请求
function post(url,postData,funsuccess) {
    ajax('post', url, true, postData, funsuccess, function () { });
}
function my$(id) {
    return document.getElementById(id);
}
// 注意：调用get传参出错是
// 可用 get('testSearch.ashx?wd=' + txt , loaddata); 
