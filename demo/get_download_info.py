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

    infos = []

    for release_info in json_obj:
        version: str = release_info["tag_name"]
        version = version.removeprefix("v")
        publish_time = release_info["published_at"]
        update_log = release_info["body"]

        infoContent = {}

        infoContent["version"] = version
        infoContent["update_time"] = publish_time
        infoContent["update_log"] = update_log

        regex = "MaaCore-(Windows|MacOS|Linux)-(x64|arm64)-v((0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?).zip"

        for asst in release_info["assets"]:

            packageInfo = infoContent.copy()

            download_url = asst["browser_download_url"]
            file_name = asst["name"]

            if re.fullmatch(regex, file_name) is None:
                continue

            info_string = file_name.replace("MaaCore-", "")
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
