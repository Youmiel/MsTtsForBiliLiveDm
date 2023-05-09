import argparse
import json
import re
import sys
from typing import Dict, List

import chardet

TEMPLATE_PATH = 'script/voice_type_template'
TYPE_JSON_PATH = 'script/voice_type.json'

TYPE_PATTERN = re.compile('.*\/\/types')
FIELD_PATTERN = re.compile('.*\/\/fields')

TEMPLATE_CODE_TYPE = '{0}public static readonly MsVoiceType {1} = new MsVoiceType("{2}","{3}");\n'
TEMPLATE_CODE_FIELD = '{0}MsVoiceType.{1}{2}\n'

def parse_args(args: List[str]) -> Dict:
    parser = argparse.ArgumentParser('vioce_type_generator', add_help=True)

    parser.add_argument('output_path')

    if len(args) < 2:
        parser.print_usage()
        sys.exit(0)

    result = parser.parse_args(args[1:])

    return {
        'output_path': result.output_path
    }

def find_types(json_obj: List[Dict]) -> List[Dict]:
    output_list = []
    for type in json_obj:
        if type['Locale'] == 'zh-CN':
            output_list.append({
                'LocalName': type['LocalName'],
                'ShortName': type['ShortName'],
                'FieldName': type['ShortName'].split('-')[-1]
            })
    return output_list
                    
def get_encoding(filename: str) -> str:
    with open(filename, 'rb') as testfile:
        result = chardet.detect(testfile.read())
        encode_type = 'utf-8' if result.get('encoding') is None \
            else result.get('encoding')
    return encode_type 

def main(output_path: str):
    # if output_path is None:
    #     sys.exit(0)

    json_encoding = get_encoding(TYPE_JSON_PATH)
    template_encoding = get_encoding(TEMPLATE_PATH)

    with open(TYPE_JSON_PATH, 'r', encoding=json_encoding) as typejsonfile:
        json_obj = json.load(typejsonfile)
    type_list = find_types(json_obj)

    codelines = []
    with open(TEMPLATE_PATH, 'r', encoding=template_encoding) as template:
        for line in template:
            if TYPE_PATTERN.match(line):
                prefix = line.split('/')[0]
                for type in type_list:
                    codelines.append(
                        TEMPLATE_CODE_TYPE.format(
                            prefix, 
                            type["FieldName"],
                            type["LocalName"],
                            type["ShortName"]))
            elif FIELD_PATTERN.match(line):
                prefix = line.split('/')[0]
                for i in range(len(type_list)):
                    codelines.append(
                        TEMPLATE_CODE_FIELD.format(
                            prefix, 
                            type_list[i]["FieldName"],
                            ',' if i < len(type_list) - 1 else ''))
            else:
                codelines.append(line)

    with open(output_path, 'w', encoding='utf-8') as codefile:
        codefile.writelines(codelines)

if __name__ == '__main__':
    kargs = parse_args(sys.argv)
    main(**kargs) 