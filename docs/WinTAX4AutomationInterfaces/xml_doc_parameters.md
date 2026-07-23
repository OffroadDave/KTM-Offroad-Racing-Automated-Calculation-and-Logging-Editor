[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# XML - Parameters

Parameters setup allows to set global settings, filters and scales applied to archive and virtual channels. These settings can be overwritten by a local window setting. **Schema:**
    
    
    <?xml version="1.0" encoding="UTF-8"?>
    <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
            <xs:element name="DecimalDigit" default="Auto">
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
            <xs:element name="FilterParameters">
                    <xs:annotation>
                            <xs:documentation>Settings for a single filter</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="Type"/>
                                    <xs:element ref="Length" minOccurs="0"/>
                                    <xs:element ref="Freq1" minOccurs="0"/>
                                    <xs:element ref="Freq2" minOccurs="0"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="FiltersParameters">
                    <xs:annotation>
                            <xs:documentation>List of filters that can be applied sequentially to channel values</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence maxOccurs="unbounded">
                                    <xs:element ref="FilterParameters" minOccurs="0"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="Freq1" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>First frequency used byt FFT-types (hertz)</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Freq2" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Second frequency used byt FFT-types (hertz)</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Gain" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Gain applied to the channel</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Length" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Filter lenght used for run-average type (msec)</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Name" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Name of the channel</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Offset" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Offset applied to the channel</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="OutputFormat" default="Auto">
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
            <xs:element name="Parameters">
                    <xs:annotation>
                            <xs:documentation>Parameters setup for a single channel - default settings used for analysis. Local window settings can ovveride these</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="Name"/>
                                    <xs:element ref="Unit" minOccurs="0"/>
                                    <xs:element ref="RangeMin" minOccurs="0"/>
                                    <xs:element ref="RangeMax" minOccurs="0"/>
                                    <xs:element ref="Offset" minOccurs="0"/>
                                    <xs:element ref="Gain" minOccurs="0"/>
                                    <xs:element ref="FiltersParameters" minOccurs="0"/>
                                    <xs:element ref="OutputFormat" minOccurs="0"/>
                                    <xs:element ref="DecimalDigit" minOccurs="0"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="ParametersArray">
                    <xs:annotation>
                            <xs:documentation>List of Parameters settings</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence maxOccurs="unbounded">
                                    <xs:element ref="Parameters" maxOccurs="unbounded"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="ParametersSetup">
                    <xs:annotation>
                            <xs:documentation>Global Parameters setup used by WinTAX</xs:documentation>
                    </xs:annotation>
                    <xs:complexType>
                            <xs:sequence>
                                    <xs:element ref="ParametersArray"/>
                                    <xs:element ref="WintaxConfiguration"/>
                            </xs:sequence>
                    </xs:complexType>
            </xs:element>
            <xs:element name="RangeMax" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Maximum scale range</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="RangeMin" type="xs:double">
                    <xs:annotation>
                            <xs:documentation>Minimum scale range</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Type" default="0">
                    <xs:annotation>
                            <xs:documentation>Filter type </xs:documentation>
                    </xs:annotation>
                    <xs:simpleType>
                            <xs:restriction base="xs:integer">
                                    <xs:enumeration value="0"/>
                                    <xs:enumeration value="1"/>
                                    <xs:enumeration value="2"/>
                                    <xs:enumeration value="3"/>
                                    <xs:enumeration value="4"/>
                                    <xs:enumeration value="5"/>
                            </xs:restriction>
                    </xs:simpleType>
            </xs:element>
            <xs:element name="Unit" type="xs:string">
                    <xs:annotation>
                            <xs:documentation>Channel Measure Unit</xs:documentation>
                    </xs:annotation>
            </xs:element>
            <xs:element name="Version">
                    <xs:complexType mixed="true"/>
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
    <ParametersSetup xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="parameters.xsd">
            <ParametersArray>
                    <Parameters>
                            <Name>StefanoTest</Name>
                            <Unit>user</Unit>
                            <RangeMin>-30.000000</RangeMin>
                            <RangeMax>0.000000</RangeMax>
                            <Offset>0.000000</Offset>
                            <Gain>1.000000</Gain>
                            <FiltersParameters>
                                    <FilterParameters>
                                            <Type>1</Type>
                                            <Length>0.200000</Length>
                                            <Freq1>0.000000</Freq1>
                                            <Freq2>0.000000</Freq2>
                                    </FilterParameters>
                            </FiltersParameters>
                            <OutputFormat>Auto</OutputFormat>
                            <DecimalDigit>Auto</DecimalDigit>
                    </Parameters>
                    <Parameters>
                            <Name>WWWWWWWWWWWWW</Name>
                            <Unit/>
                            <RangeMin>-29.000000</RangeMin>
                            <RangeMax>1.000000</RangeMax>
                            <Offset>0.000000</Offset>
                            <Gain>1.000000</Gain>
                            <FiltersParameters/>
                            <OutputFormat>Auto</OutputFormat>
                            <DecimalDigit>Auto</DecimalDigit>
                    </Parameters>
            </ParametersArray>
            <WintaxConfiguration>
                    <Version>3.00</Version>
            </WintaxConfiguration>
    </ParametersSetup>
    