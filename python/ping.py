#!/usr/bin/env python
"""
A simple test to ping seafile web api.
"""

import urllib2

req = urllib2.Request('https://seacloud.cc/api2/ping/')
response = urllib2.urlopen(req)
the_page = response.read()

print the_page

