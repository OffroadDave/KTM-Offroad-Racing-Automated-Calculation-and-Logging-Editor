[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# XML - Virtual Channels

Virtual Channel setup manages virtual channels based on math expressions. **Schema:**
    
    
    <?xml version="1.0" encoding="UTF-8"?>
    <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
            <xs:element name="Comment" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Channel description</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Decimals" default="Auto">
                    <xs:annotation>
                            <xs:documentation>Number of decimal digits used in analysis</xs:documentation>
                    </xs:annotation>
                    <xs:simpleType>
                            <xs:restriction base="xs:string">
                                    <xs:enumeration value="Auto"/>
                                    <xs:enumeration value="0"/>
                                    <xs:enumeration value="1"/>
                                    <xs:enumeration value="2"/>
                                    <xs:enumeration value="3"/>
                                    <xs:enumeration value="4"/>
                                    <xs:enumeration value="5"/>
                                    <xs:enumeration value="6"/>
                                    <xs:enumeration value="7"/>
                                    <xs:enumeration value="8"/>
                                    <xs:enumeration value="9"/>
                                    <xs:enumeration value="10"/>
                                    <xs:enumeration value="11"/>
                                    <xs:enumeration value="12"/>
                                    <xs:enumeration value="13"/>
                                    <xs:enumeration value="14"/>
                                    <xs:enumeration value="15"/>
                            </xs:restriction>
                    </xs:simpleType>
            </xs:element>
            <xs:element name="Expression" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Math expression</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Freq">
                    <xs:annotation>
                            <xs:documentation>Math channel frequency - use Auto for an automatic search</xs:documentation>
                    </xs:annotation>
                    <xs:simpleType>
                            <xs:restriction base="xs:string">
                                    <xs:enumeration value="Auto"/>
                                    <xs:enumeration value="off"/>
                                    <xs:enumeration value="1 Hz"/>
                                    <xs:enumeration value="2 Hz"/>
                                    <xs:enumeration value="5 Hz"/>
                                    <xs:enumeration value="10 Hz"/>
                                    <xs:enumeration value="20 Hz"/>
                                    <xs:enumeration value="50 Hz"/>
                                    <xs:enumeration value="100 Hz"/>
                                    <xs:enumeration value="200 Hz"/>
                                    <xs:enumeration value="500 Hz"/>
                                    <xs:enumeration value="1 kHz"/>
                                    <xs:enumeration value="2 kHz"/>
                                    <xs:enumeration value="5 kHz"/>
                                    <xs:enumeration value="10 kHz"/>
                            </xs:restriction>
                    </xs:simpleType>
            </xs:element>
            <xs:element name="Library">
                    <xs:annotation>
                            <xs:documentation>Single VCH library used by WinTAX - contains single VCHs</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="VirtualChannels" minOccurs="0"/>
                                    <xs:element ref="WintaxConfiguration"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="Measure" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Measure Unit</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Name" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Channel Name</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="OutFormat" default="Auto">
                    <xs:annotation>
                            <xs:documentation>Output format used to display channel values</xs:documentation>
                    </xs:annotation>
                    <xs:simpleType>
                            <xs:restriction base="xs:string">
                                    <xs:enumeration value="Auto"/>
                                    <xs:enumeration value="Dec"/>
                                    <xs:enumeration value="Numeric"/>
                                    <xs:enumeration value="Scientific"/>
                                    <xs:enumeration value="Hex"/>
                                    <xs:enumeration value="Bin"/>
                                    <xs:enumeration value="Ascii"/>
                            </xs:restriction>
                    </xs:simpleType>
            </xs:element>
            <xs:element name="Properties">
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="Comment" minOccurs="0"/>
                                    <xs:element ref="Measure" minOccurs="0"/>
                                    <xs:element ref="Expression"/>
                                    <xs:element ref="Freq"/>
                                    <xs:element ref="OutFormat"/>
                                    <xs:element ref="Decimals"/>
                                    <xs:element ref="Showhide" minOccurs="0"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="Showhide">
                    <xs:annotation>
                            <xs:documentation>Flag used to display or hide the channel in WinTAX browser</xs:documentation>
                    </xs:annotation>
                    <xs:simpleType>
                            <xs:restriction base="xs:integer">
                                    <xs:enumeration value="0"/>
                                    <xs:enumeration value="1"/>
                            </xs:restriction>
                    </xs:simpleType>
            </xs:element>
            <xs:element name="VCH">
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="Name"/>
                                    <xs:element ref="Properties"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="Version">
                    <xs:complexType mixed="true"/>
            </xs:element>
            <xs:element name="VirtualChannels">
                    <xs:annotation>
                            <xs:documentation>List of virtual channels</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="VCH" minOccurs="0" maxOccurs="unbounded"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="WintaxConfiguration">
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="Version"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
    </xs:schema>
    

**Sample:**
    
    
    <?xml version="1.0"?>
    <Library xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="virtualchannels.xsd">
            <VirtualChannels>
                    <VCH>
                            <Name>CanaleDiTest</Name>
                            <Properties>
                                    <Comment/>
                                    <Measure>udm</Measure>
                                    <Expression>SIN(TIME)</Expression>
                                    <Freq>10 kHz</Freq>
                                    <OutFormat>Dec</OutFormat>
                                    <Decimals>5</Decimals>
                                    <Showhide>1</Showhide>
                            </Properties>
                    </VCH>
                    <VCH>
                            <Name>Test_NumFloat_auto_dec</Name>
                            <Properties>
                                    <Comment/>
                                    <Measure>g</Measure>
                                    <Expression>$TIME + sin($TIME)</Expression>
                                    <Freq>Auto</Freq>
                                    <OutFormat>Dec</OutFormat>
                                    <Decimals>Auto</Decimals>
                                    <Showhide>1</Showhide>
                            </Properties>
                    </VCH>
            </VirtualChannels>
            <WintaxConfiguration>
                    <Version>3.00</Version>
            </WintaxConfiguration>
    </Library>
    