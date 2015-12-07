#!/usr/bin/env python
# -*- coding: utf-8 -*-
import requests

def get_upload_link(url, token):
    resp = requests.get(
        url, headers={'Authorization': 'Token {token}'. format(token=token)}
    )
    return resp.json()

if __name__ == "__main__":
    # replace with your token
    token = '059d2751e07ad30d1e0422bea4fbe06367f40b84'
    # replace with your library id
    upload_link = get_upload_link(
        'https://seacloud.cc/api2/repos/7edf2f99-ea8d-4b40-9f9b-63928b2c9477/upload-link/', token
    )

    response = requests.post(
        upload_link, data={'filename': 'git.jpg', 'parent_dir': '/'},
        files={'file': open('/Users/xiez/Pictures/git.jpg', 'rb')},
        headers={'Authorization': 'Token {token}'. format(token=token)}
    )
    print response
