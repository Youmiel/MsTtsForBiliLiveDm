import json
from typing import Dict, List
import requests

OUTPUT_JSON_PATH = 'script/voice_names.json'
API_PATTERN = 'https://{}.api.speech.microsoft.com/cognitiveservices/voices/list'
LOCATION_LIST = [
    'westus',
    'westus2',
    'southcentralus',
    'westcentralus',
    'eastus',
    'eastus2',
    'westeurope',
    'northeurope',
    'southbrazil',
    'eastaustralia',
    'southeastasia',
    'eastasia'
]

def main():
    location_type_dict = {}
    for name in LOCATION_LIST:
        print(name)
        name_list = []
        url = API_PATTERN.format(name)
        try:
            response = requests.get(url, headers={'Origin': 'https://azure.microsoft.com'})

            list_json = response.text
            voice_type_list : List[Dict] = json.loads(list_json)

            for voice_type in voice_type_list:
                if voice_type['Locale'] == 'zh-CN':
                    name_list.append(voice_type['LocalName'])
            name_list.sort()
        except Exception as e:
            print('error happens when requesting')

        location_type_dict[name] = name_list
    
    # find mutual names
    universal_set = set()
    missing_set = set()
    for key in location_type_dict:
        if len(location_type_dict[key]) > 0:
            universal_set.update(location_type_dict[key])

    for key in location_type_dict:
        if len(location_type_dict[key]) > 0:
            missing_set.update(universal_set.difference(location_type_dict[key]))

    location_type_dict['mutual'] = list(universal_set.difference(missing_set))
    location_type_dict['mutual'].sort()
    print()
    print( location_type_dict['mutual'])
    
    with open(OUTPUT_JSON_PATH, 'w', encoding='utf-8') as jsonfile:
        json.dump(location_type_dict, jsonfile, indent=2, ensure_ascii=False)

if __name__ == '__main__':
    main()