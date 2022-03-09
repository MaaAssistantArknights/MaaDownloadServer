from itertools import count
import sys
import json
import re

if len(sys.argv) != 3:
    sys.stderr.write("参数不正确, 当前参数共" + str(len(sys.argv)) +
                     "个, 为: " + str(sys.argv) + "\n")

config_json_string = sys.argv[1]
json_string = sys.argv[2]

try:
    json_obj = json.loads(json_string)
    config = json.loads(config_json_string)

    version: str = json_obj["tag_name"]
    version = version.removeprefix("v")
    publish_time = json_obj["published_at"]
    update_log = json_obj["body"]

    infos = []

    infoContent = {}

    infoContent["version"] = version
    infoContent["update_time"] = publish_time
    infoContent["update_log"] = update_log

    regex = config["name"] + "-(.+)-(.+).zip"

    for asst in json_obj["assets"]:

        packageInfo = infoContent.copy()

        download_url = asst["browser_download_url"]
        file_name = asst["name"]

        if re.fullmatch(regex, file_name) is None:
            continue

        info_string = file_name.replace(config["name"] + "-", "")
        splitted = info_string.split("-")
        platform_string = splitted[0]
        arch_string = splitted[1]

        packageInfo["platform"] = platform_string
        packageInfo["arch"] = arch_string
        packageInfo["download_url"] = download_url
        packageInfo["file_extension"] = "zip"
        packageInfo["checksum"] = ""
        packageInfo["checksum_type"] = "none"

        infos.append(packageInfo)

    json_output = json.dumps(infos)

    sys.stdout.write(json_output)

except Exception as e:
    sys.stderr.write(str(e) + "\n")
