#!/usr/bin/env python
"""
Fetch a specific library information.
"""

import urllib2

# replace with your token
token = '4da16bfbba92546df05f6ecfba68449be7a9f456'

repo_id = 'e70c28cb-3be2-4aef-a464-915ba107f907'

url = 'https://seacloud.cc/api2/repos/%s/' % repo_id

req = urllib2.Request(url)
req.add_header('Authorization', 'Token ' + token)
response = urllib2.urlopen(req)
the_page = response.read()

print the_page
