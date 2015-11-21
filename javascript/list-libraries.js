// replace with your own token
var token = '9dfa7a1f2dfa1c97550e55469bca9a68dcc7e03c'

var invocation = new XMLHttpRequest();
var url = 'https://seacloud.cc/api2/repos/';
function listLibraries() {
    if(invocation) {    
        invocation.open('GET', url, true);
        invocation.setRequestHeader("Authorization", "Token " + token);
        invocation.onreadystatechange = function() {
            if(invocation.readyState == 4 && invocation.status == 200) {
                console.log(invocation.responseText);
            }
        };

        invocation.send();
    }
}

listLibraries();
