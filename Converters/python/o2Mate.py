#!/usr/bin/env python

import re
import cStringIO
import defusedexpat
import xmltodict
import dpath.util
import os

class o2MateExpr:
    def __init__(self, value):
        self.value = value
    
    def __add__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        return int(xa) + int(xb)
        
    def concat(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        return str(xa) + str(xb)

    def __sub__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        return int(xa) - int(xb)

    def __mul__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        return int(xa) * int(xb)

    def __div__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        if xb == 0:
            return 0
        return int(xa) / int(xb)
        
        
    def __lt__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        return int(xa) < int(xb)
        
    def __gt__(a,b):
        xa = xb = 0
        xa = a.value
        if isinstance(b, a.__class__):
            xb = b.value
        else:
            xb = b
        if type(xa) is str and not re.search('^\d+$', xa):
            xa = 0
        if type(xb) is str and not re.search('^\d+$', xb):
            xb = 0
        return int(xa) > int(xb)

    def __eq__(a,b):
        sa = sb = 0
        ia = ib = 0
        sa = a.value
        if isinstance(b, a.__class__):
            sb = b.value
        else:
            sb = b
        if type(sa) is str and not re.search('^\d+$', sa):
            ia = 0
        else:
            ia = int(sa)
        if type(sb) is str and not re.search('^\d+$', sb):
            ib = 0
        else:
            ib = int(sb)
        if sa is sb:
            return sa == sb
        else:
            return ia == ib
                
    def __getitem__(self, index):
        return str(self.value)[index - 1]
        
class o2MateDict:

    def __init__(self, fileName):
        input = open(fileName, 'r')
        self.dict = xmltodict.parse(input.read())
        input.close()
        
    def getString(self, name):
        for (key, value) in dpath.util.search(self.dict, u'/Dictionnaire/value/*', yielded=True):
            if value[u'@type'] == u'string' and value[u'@name'] == name:
                return value[u'#text']
        return ""

    def getField(self, name, index, fieldName):
        for (key, value) in dpath.util.search(self.dict, u'/Dictionnaire/value/*', yielded=True):
            if value[u'@type'] == u'array' and value[u'@name'] == name:
                for (keyField, valueField) in dpath.util.search(value[u'item'], str(index-1) + u'/field/*', yielded=True):
                    if valueField[u'@name'] == fieldName:
                        return valueField[u'#text']
        return ""
                    
        