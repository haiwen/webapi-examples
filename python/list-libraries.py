#!/usr/bin/env python
"""
List libraries including owned, shared and group libraries.
"""

import urllib2

# replace with your token
token = '4da16bfbba92546df05f6ecfba68449be7a9f456'

url = 'https://seacloud.cc/api2/repos/'

req = urllib2.Request(url)
req.add_header('Authorization', 'Token ' + token)
response = urllib2.urlopen(req)
the_page = response.read()

print the_page
