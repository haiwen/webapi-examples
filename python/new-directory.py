#!/usr/bin/env python
"""
Make a directory.
"""

import urllib
import urllib2

# replace with your token
token = '4da16bfbba92546df05f6ecfba68449be7a9f456'

repo_id = 'dfec548c-0d52-4e9f-b455-0e3aecf83498'

url = 'https://seacloud.cc/api2/repos/%s/dir/?p=/foo' % repo_id
values = {'operation': 'mkdir'}
data = urllib.urlencode(values)
headers = {'Authorization': 'Token ' + token}

req = urllib2.Request(url, data, headers)
response = urllib2.urlopen(req)
the_page = response.read()

print the_page
