<?xml version="1.0"?> 
 <drawfbp_file xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
xsi:noNamespaceSchemaLocation="https://github.com/jpaulm/drawfbp/blob/master/lib/drawfbp_file.xsd">
<net>
<desc>Click anywhere on selection area</desc>
 <blocks><block> <x> 727 </x> <y> 247 </y> <id> 1 </id> <type>B</type> <width>92</width> <height>64</height> <description>TimeSeries</description> <compname>ConnectToSturdyRef.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/ConnectToSturdyRef.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 955 </x> <y> 484 </y> <id> 2 </id> <type>B</type> <width>92</width> <height>64</height> <description>Run Monica</description> <compname>RunEnvModel.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/RunEnvModel.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 1199 </x> <y> 484 </y> <id> 3 </id> <type>B</type> <width>92</width> <height>64</height> <description>WriteConsole</description> <compname>WriteToConsole.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/FBPVerbsCore/FBPComponents/WriteToConsole.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 730 </x> <y> 106 </y> <id> 4 </id> <type>I</type> <width>188</width> <height>29</height> <description>capnp://localhost:11002</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 666 </x> <y> 156 </y> <id> 5 </id> <type>I</type> <width>84</width> <height>29</height> <description>TimeSeries</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 943 </x> <y> 243 </y> <id> 10 </id> <type>B</type> <width>92</width> <height>64</height> <description>MONICA</description> <compname>ConnectToSturdyRef.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/ConnectToSturdyRef.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 962 </x> <y> 100 </y> <id> 11 </id> <type>I</type> <width>180</width> <height>29</height> <description>capnp://localhost:6666</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 1133 </x> <y> 150 </y> <id> 12 </id> <type>I</type> <width>340</width> <height>29</height> <description>EnvInstance\<StructuredText,StructuredText\></description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 727 </x> <y> 484 </y> <id> 14 </id> <type>B</type> <width>92</width> <height>64</height> <description>Create Env</description> <compname>CreateModelEnv.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/CreateModelEnv.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 366 </x> <y> 503 </y> <id> 15 </id> <type>B</type> <width>92</width> <height>64</height> <description>Create JSON
Rest Env</description> <compname>CreateMonicaEnv.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/CreateMonicaEnv.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 178 </x> <y> 422 </y> <id> 16 </id> <type>B</type> <width>92</width> <height>64</height> <description>Read
sim.json</description> <compname>ReadJsonObject.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/ReadJsonObject.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 178 </x> <y> 530 </y> <id> 17 </id> <type>B</type> <width>92</width> <height>64</height> <description>Read
crop.json</description> <compname>ReadJsonObject.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/ReadJsonObject.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 178 </x> <y> 637 </y> <id> 18 </id> <type>B</type> <width>92</width> <height>64</height> <description>Read
site.json</description> <compname>ReadJsonObject.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/ReadJsonObject.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> -174 </x> <y> 413 </y> <id> 19 </id> <type>I</type> <width>444</width> <height>29</height> <description>C:\Users\micha\MONICA\Examples\Hohenfinow2\sim-min.json</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> -171 </x> <y> 528 </y> <id> 20 </id> <type>I</type> <width>452</width> <height>29</height> <description>C:\Users\micha\MONICA\Examples\Hohenfinow2\crop-min.json</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> -200 </x> <y> 637 </y> <id> 21 </id> <type>I</type> <width>452</width> <height>29</height> <description>C:\Users\micha\MONICA\Examples\Hohenfinow2\site-min.json</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 540 </x> <y> 498 </y> <id> 23 </id> <type>B</type> <width>92</width> <height>64</height> <description>Make
Structured
Text</description> <compname>CreateStructuredText.cs</compname> <blockclassname>C:/Users/micha/GitHub/csharpfbp/MonicaFlow/Components/CreateStructuredText.cs</blockclassname> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
<block> <x> 473 </x> <y> 402 </y> <id> 24 </id> <type>I</type> <width>36</width> <height>29</height> <description>JSON</description> <multiplex>false</multiplex><invisible>false</invisible><issubnet>false</issubnet> 
</block> 
</blocks> <connections>
<connection> <fromx>1001</fromx> <fromy>481</fromy> <tox>1153</tox> <toy>483</toy> <fromid>2</fromid> <toid>3</toid> <id>3</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>IN</downstreamport><segno>0</segno></connection> 
<connection> <fromx>735</fromx> <fromy>120</fromy> <tox>754</tox> <toy>215</toy> <fromid>4</fromid> <toid>1</toid> <id>8</id> <endsatline>false</endsatline><downstreamport>SR</downstreamport><segno>0</segno></connection> 
<connection> <fromx>909</fromx> <fromy>114</fromy> <tox>917</tox> <toy>211</toy> <fromid>11</fromid> <toid>10</toid> <id>10</id> <endsatline>false</endsatline><downstreamport>SR</downstreamport><segno>0</segno></connection> 
<connection> <fromx>946</fromx> <fromy>275</fromy> <tox>948</tox> <toy>452</toy> <fromid>10</fromid> <toid>2</toid> <id>12</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>CAP</downstreamport><segno>0</segno></connection> 
<connection> <fromx>773</fromx> <fromy>486</fromy> <tox>909</tox> <toy>486</toy> <fromid>14</fromid> <toid>2</toid> <id>13</id> <endsatline>false</endsatline><upstreamport>ENV</upstreamport><downstreamport>ENV</downstreamport><segno>0</segno></connection> 
<connection> <fromx>224</fromx> <fromy>429</fromy> <tox>320</tox> <toy>481</toy> <fromid>16</fromid> <toid>15</toid> <id>16</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>SIM</downstreamport><segno>0</segno></connection> 
<connection> <fromx>224</fromx> <fromy>534</fromy> <tox>320</tox> <toy>505</toy> <fromid>17</fromid> <toid>15</toid> <id>17</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>CROP</downstreamport><segno>0</segno></connection> 
<connection> <fromx>224</fromx> <fromy>639</fromy> <tox>320</tox> <toy>525</toy> <fromid>18</fromid> <toid>15</toid> <id>18</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>SITE</downstreamport><segno>0</segno></connection> 
<connection> <fromx>48</fromx> <fromy>413</fromy> <tox>132</tox> <toy>424</toy> <fromid>19</fromid> <toid>16</toid> <id>19</id> <endsatline>false</endsatline><downstreamport>IN</downstreamport><segno>0</segno></connection> 
<connection> <fromx>55</fromx> <fromy>530</fromy> <tox>132</tox> <toy>530</toy> <fromid>20</fromid> <toid>17</toid> <id>20</id> <endsatline>false</endsatline><downstreamport>IN</downstreamport><segno>0</segno></connection> 
<connection> <fromx>26</fromx> <fromy>639</fromy> <tox>132</tox> <toy>639</toy> <fromid>21</fromid> <toid>18</toid> <id>21</id> <endsatline>false</endsatline><downstreamport>IN</downstreamport><segno>0</segno></connection> 
<connection> <fromx>412</fromx> <fromy>504</fromy> <tox>494</tox> <toy>504</toy> <fromid>15</fromid> <toid>23</toid> <id>22</id> <endsatline>false</endsatline><upstreamport>ENV</upstreamport><downstreamport>IN</downstreamport><segno>0</segno></connection> 
<connection> <fromx>482</fromx> <fromy>416</fromy> <tox>520</tox> <toy>466</toy> <fromid>24</fromid> <toid>23</toid> <id>23</id> <endsatline>false</endsatline><downstreamport>STR</downstreamport><segno>0</segno></connection> 
<connection> <fromx>586</fromx> <fromy>504</fromy> <tox>681</tox> <toy>495</toy> <fromid>23</fromid> <toid>14</toid> <id>24</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>REST</downstreamport><segno>0</segno></connection> 
<connection> <fromx>1027</fromx> <fromy>164</fromy> <tox>968</tox> <toy>211</toy> <fromid>12</fromid> <toid>10</toid> <id>25</id> <endsatline>false</endsatline><downstreamport>CT</downstreamport><segno>0</segno></connection> 
<connection> <fromx>708</fromx> <fromy>169</fromy> <tox>699</tox> <toy>215</toy> <fromid>5</fromid> <toid>1</toid> <id>26</id> <endsatline>false</endsatline><downstreamport>CT</downstreamport><segno>0</segno></connection> 
<connection> <fromx>733</fromx> <fromy>279</fromy> <tox>733</tox> <toy>452</toy> <fromid>1</fromid> <toid>14</toid> <id>27</id> <endsatline>false</endsatline><upstreamport>OUT</upstreamport><downstreamport>TS</downstreamport><segno>0</segno></connection> 
</connections> </net> </drawfbp_file>