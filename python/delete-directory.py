#!/usr/bin/env python
"""
Delete a directory.
"""

import urllib2

# replace with your token
token = '4da16bfbba92546df05f6ecfba68449be7a9f456'

repo_id = 'dfec548c-0d52-4e9f-b455-0e3aecf83498'

url = 'https://seacloud.cc/api2/repos/%s/dir/?p=/foo' % repo_id

req = urllib2.Request(url)
req.add_header('Authorization', 'Token ' + token)
req.get_method = lambda: 'DELETE'
response = urllib2.urlopen(req)
the_page = response.read()

print the_page
