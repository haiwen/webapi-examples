#!/usr/bin/env python
"""
Post seafile email/password to obtain auth token.
"""

import urllib
import urllib2
import simplejson as json


url = 'https://seacloud.cc/api2/auth-token/'
values = {'username': 'demo@seafile.com',
          'password': 'demo'}
data = urllib.urlencode(values)
req = urllib2.Request(url, data)
response = urllib2.urlopen(req)
the_page = response.read()
token = json.loads(the_page)['token']

print token

