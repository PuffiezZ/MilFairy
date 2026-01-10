//Maya ASCII 2024 scene
//Name: Buggy.ma
//Last modified: Sat, Jan 10, 2026 04:34:54 PM
//Codeset: 874
requires maya "2024";
requires -nodeType "aiOptions" -nodeType "aiAOVDriver" -nodeType "aiAOVFilter" "mtoa" "5.3.0";
currentUnit -l centimeter -a degree -t film;
fileInfo "application" "maya";
fileInfo "product" "Maya 2024";
fileInfo "version" "2024";
fileInfo "cutIdentifier" "202302170737-4500172811";
fileInfo "osv" "Windows 11 Home v2009 (Build: 26200)";
fileInfo "UUID" "0215F746-42B0-E958-41B3-E485F0D7BCCB";
fileInfo "license" "education";
createNode transform -s -n "persp";
	rename -uid "D4203884-4942-1194-9D42-E1B7B88A79ED";
	setAttr ".v" no;
	setAttr ".t" -type "double3" -5.2323584800882905 19.873540101998071 1.2151714290404336 ;
	setAttr ".r" -type "double3" -111.00000000000526 106.00000000000036 2.5444437451708134e-14 ;
	setAttr ".rpt" -type "double3" 2.0319046858665308e-16 9.0887301258799979e-17 -9.768138894859365e-18 ;
createNode camera -s -n "perspShape" -p "persp";
	rename -uid "14B36E54-4B86-B9E6-250A-E2888ECEF084";
	setAttr -k off ".v" no;
	setAttr ".fl" 34.999999999999979;
	setAttr ".coi" 16.692951929717655;
	setAttr ".imn" -type "string" "persp";
	setAttr ".den" -type "string" "persp_depth";
	setAttr ".man" -type "string" "persp_mask";
	setAttr ".tp" -type "double3" 1.674720233322105 6.32688671196592 0.67448453473779657 ;
	setAttr ".hc" -type "string" "viewSet -p %camera";
createNode transform -s -n "top";
	rename -uid "A0B4060E-4748-874C-4A6F-84882A442B9F";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 0 1000.1 0 ;
	setAttr ".r" -type "double3" -90 0 0 ;
createNode camera -s -n "topShape" -p "top";
	rename -uid "6F60EC4B-4E7E-1C9A-99BD-65B1095E5F04";
	setAttr -k off ".v" no;
	setAttr ".rnd" no;
	setAttr ".coi" 1000.1;
	setAttr ".ow" 34.395604395604394;
	setAttr ".imn" -type "string" "top";
	setAttr ".den" -type "string" "top_depth";
	setAttr ".man" -type "string" "top_mask";
	setAttr ".hc" -type "string" "viewSet -t %camera";
	setAttr ".o" yes;
	setAttr ".ai_translator" -type "string" "orthographic";
createNode transform -s -n "front";
	rename -uid "897D1428-424D-36B6-56B1-288AEF41116D";
	setAttr ".t" -type "double3" -0.13483712195613395 3.7716862036706402 1000.1 ;
createNode camera -s -n "frontShape" -p "front";
	rename -uid "2408ACD8-4BDD-E86A-14F2-1E853BD02762";
	setAttr -k off ".v";
	setAttr ".rnd" no;
	setAttr ".coi" 1000.1;
	setAttr ".ow" 23.885089749866459;
	setAttr ".imn" -type "string" "front";
	setAttr ".den" -type "string" "front_depth";
	setAttr ".man" -type "string" "front_mask";
	setAttr ".hc" -type "string" "viewSet -f %camera";
	setAttr ".o" yes;
	setAttr ".ai_translator" -type "string" "orthographic";
createNode transform -s -n "side";
	rename -uid "016E7418-4241-8B0B-89F7-F6B1352EA77B";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 1000.1 4.5775432345067646 1.0840866912947593 ;
	setAttr ".r" -type "double3" 0 90 0 ;
createNode camera -s -n "sideShape" -p "side";
	rename -uid "9AEB23FE-4BCD-827B-BF3E-3299CCAED398";
	setAttr -k off ".v" no;
	setAttr ".rnd" no;
	setAttr ".coi" 1000.1;
	setAttr ".ow" 4.5894085161448741;
	setAttr ".imn" -type "string" "side";
	setAttr ".den" -type "string" "side_depth";
	setAttr ".man" -type "string" "side_mask";
	setAttr ".hc" -type "string" "viewSet -s %camera";
	setAttr ".o" yes;
	setAttr ".ai_translator" -type "string" "orthographic";
createNode transform -n "imagePlane1";
	rename -uid "42333D97-44B4-AD86-6386-5AB3E86BB567";
	setAttr ".t" -type "double3" 0.2074569874002451 4 -12 ;
	setAttr -l on ".tx";
	setAttr -l on ".ty";
	setAttr -l on ".tz";
	setAttr -l on ".rx";
	setAttr -l on ".ry";
	setAttr -l on ".rz";
	setAttr -l on ".sx";
	setAttr -l on ".sy";
	setAttr -l on ".sz";
createNode imagePlane -n "imagePlaneShape1" -p "imagePlane1";
	rename -uid "580F80EF-4100-A8EE-C34B-FABE62371B9E";
	setAttr -k off ".v";
	setAttr ".fc" 203;
	setAttr ".imn" -type "string" "C:/3D_Model/LMAGE/Buggy01.png";
	setAttr ".cov" -type "short2" 800 800 ;
	setAttr ".dlc" no;
	setAttr ".w" 8;
	setAttr ".h" 8;
	setAttr ".cs" -type "string" "sRGB";
createNode transform -n "pCube1";
	rename -uid "98325210-4F2E-46EF-5457-3FADD814B57E";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 0 3.9523086896213799 -1.1628316454090801e-17 ;
	setAttr ".s" -type "double3" 2.1536810012044096 2.1536810012044096 2.1536810012044096 ;
	setAttr ".spt" -type "double3" 0 0 -1.719118656522644e-08 ;
createNode mesh -n "pCubeShape1" -p "pCube1";
	rename -uid "6C27E9F9-45B0-E7CA-685A-AF8C116D3396";
	setAttr -k off ".v";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 6 ".gtag";
	setAttr ".gtag[0].gtagnm" -type "string" "back";
	setAttr ".gtag[0].gtagcmp" -type "componentList" 2 "f[32:47]" "f[112:123]";
	setAttr ".gtag[1].gtagnm" -type "string" "bottom";
	setAttr ".gtag[1].gtagcmp" -type "componentList" 2 "f[48:63]" "f[124:131]";
	setAttr ".gtag[2].gtagnm" -type "string" "front";
	setAttr ".gtag[2].gtagcmp" -type "componentList" 1 "f[0:15]";
	setAttr ".gtag[3].gtagnm" -type "string" "left";
	setAttr ".gtag[3].gtagcmp" -type "componentList" 2 "f[80:95]" "f[144:159]";
	setAttr ".gtag[4].gtagnm" -type "string" "right";
	setAttr ".gtag[4].gtagcmp" -type "componentList" 2 "f[64:79]" "f[132:143]";
	setAttr ".gtag[5].gtagnm" -type "string" "top";
	setAttr ".gtag[5].gtagcmp" -type "componentList" 2 "f[16:31]" "f[96:111]";
	setAttr ".pv" -type "double2" 0.40625 0.59375 ;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr -s 189 ".uvst[0].uvsp[0:188]" -type "float2" 0.375 0 0.375 1 0.625
		 0 0.625 1 0.375 0.25 0.625 0.25 0.375 0.5 0.125 0.25 0.625 0.5 0.875 0.25 0.375 0.75
		 0.125 0 0.625 0.75 0.875 0 0.5 0.125 0.5 0.375 0.5 0.625 0.5 0.875 0.75 0.125 0.25
		 0.125 0.5 0 0.5 1 0.625 0.125 0.5 0.25 0.375 0.125 0.625 0.375 0.75 0.25 0.5 0.5
		 0.375 0.375 0.25 0.25 0.625 0.625 0.875 0.125 0.5 0.75 0.375 0.625 0.125 0.125 0.625
		 0.875 0.75 0 0.375 0.875 0.25 0 0.4375 0.0625 0.5625 0.0625 0.5625 0.1875 0.4375
		 0.1875 0.4375 0.3125 0.5625 0.3125 0.5625 0.4375 0.4375 0.4375 0.4375 0.5625 0.5625
		 0.5625 0.5625 0.6875 0.4375 0.6875 0.4375 0.8125 0.5625 0.8125 0.5625 0.9375 0.4375
		 0.9375 0.6875 0.0625 0.8125 0.0625 0.8125 0.1875 0.6875 0.1875 0.1875 0.0625 0.3125
		 0.0625 0.3125 0.1875 0.1875 0.1875 0.5 0.0625 0.5625 0.125 0.5 0.1875 0.4375 0.125
		 0.5 0.3125 0.5625 0.375 0.5 0.4375 0.4375 0.375 0.5 0.5625 0.5625 0.625 0.5 0.6875
		 0.4375 0.625 0.5 0.8125 0.5625 0.875 0.5 0.9375 0.4375 0.875 0.75 0.0625 0.8125 0.125
		 0.75 0.1875 0.6875 0.125 0.25 0.0625 0.3125 0.125 0.25 0.1875 0.1875 0.125 0.4375
		 0 0.4375 1 0.5625 0 0.5625 1 0.625 0.0625 0.625 0.1875 0.5625 0.25 0.4375 0.25 0.375
		 0.1875 0.375 0.0625 0.625 0.3125 0.6875 0.25 0.625 0.4375 0.8125 0.25 0.5625 0.5
		 0.4375 0.5 0.375 0.4375 0.1875 0.25 0.375 0.3125 0.3125 0.25 0.625 0.5625 0.875 0.1875
		 0.625 0.6875 0.875 0.0625 0.5625 0.75 0.4375 0.75 0.375 0.6875 0.125 0.0625 0.375
		 0.5625 0.125 0.1875 0.625 0.8125 0.8125 0 0.625 0.9375 0.6875 0 0.375 0.9375 0.3125
		 0 0.375 0.8125 0.1875 0 0.4375 0.25 0.5 0.25 0.5 0.3125 0.4375 0.3125 0.5625 0.3125
		 0.625 0.3125 0.625 0.375 0.5625 0.375 0.5625 0.4375 0.625 0.4375 0.625 0.5 0.5625
		 0.5 0.4375 0.375 0.5 0.375 0.5 0.4375 0.4375 0.4375 0.4375 0.625 0.375 0.625 0.375
		 0.5625 0.4375 0.5625 0.5625 0.625 0.5 0.625 0.5 0.5625 0.5625 0.5625 0.5 0.6875 0.5625
		 0.6875 0.5625 0.75 0.5 0.75 0.5 0.875 0.4375 0.875 0.4375 0.8125 0.5 0.8125 0.5625
		 0.9375 0.625 0.9375 0.625 1 0.5625 1 0.625 0.0625 0.6875 0.0625 0.6875 0.125 0.625
		 0.125 0.75 0.0625 0.75 0 0.8125 0 0.8125 0.0625 0.75 0.1875 0.75 0.125 0.8125 0.125
		 0.8125 0.1875 0.125 0 0.1875 0 0.1875 0.0625 0.125 0.0625 0.3125 0 0.3125 0.0625
		 0.25 0.0625 0.25 0 0.3125 0.1875 0.3125 0.25 0.25 0.25 0.25 0.1875 0.25 0.125 0.1875
		 0.1875 0.1875 0.125 0.25 0.1875;
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr -s 68 ".pt";
	setAttr ".pt[13]" -type "float3" -0.042745929 -0.060580485 0.0026503119 ;
	setAttr ".pt[46]" -type "float3" -0.0507458 -0.036671471 -0.025009245 ;
	setAttr ".pt[70]" -type "float3" -0.055623341 -0.041400142 0.0026502979 ;
	setAttr ".pt[97]" -type "float3" -0.03907387 -0.055961277 -0.023289105 ;
	setAttr ".pt[98]" -type "float3" 0.12264797 0.12350863 -0.057555217 ;
	setAttr ".pt[99]" -type "float3" -0.16279405 0.081676289 -0.09938731 ;
	setAttr ".pt[100]" -type "float3" -0.16279404 -0.060353413 0.13442047 ;
	setAttr ".pt[101]" -type "float3" 0.14810383 -0.0095182089 0.15666099 ;
	setAttr ".pt[102]" -type "float3" -0.035170197 0.2230235 -0.1302917 ;
	setAttr ".pt[103]" -type "float3" 0.23698066 0.058345117 -0.15455984 ;
	setAttr ".pt[104]" -type "float3" -0.086984105 0.16560975 0.2292541 ;
	setAttr ".pt[105]" -type "float3" 0.2146368 -0.0419272 0.2292541 ;
	setAttr ".pt[106]" -type "float3" -0.11528959 0.06903889 -0.12812987 ;
	setAttr ".pt[107]" -type "float3" 0.12997237 -0.079368286 -0.10625932 ;
	setAttr ".pt[108]" -type "float3" -0.037499435 0.13480234 0.015251393 ;
	setAttr ".pt[109]" -type "float3" 0.15500227 0.051457386 0.098596208 ;
	setAttr ".pt[110]" -type "float3" -0.16788566 -0.033509769 -0.18657075 ;
	setAttr ".pt[111]" -type "float3" 0.15292694 0.018195124 -0.18657072 ;
	setAttr ".pt[112]" -type "float3" -0.16788568 0.057931911 0.18119751 ;
	setAttr ".pt[113]" -type "float3" 0.1331009 0.10714819 0.15966542 ;
	setAttr ".pt[114]" -type "float3" -0.1479373 0.18209776 0.10124014 ;
	setAttr ".pt[115]" -type "float3" 0.12521011 0.18209776 -0.095645003 ;
	setAttr ".pt[116]" -type "float3" 0.079038285 -0.14795879 -0.15228464 ;
	setAttr ".pt[117]" -type "float3" -0.16850309 -0.1648002 -0.0024980477 ;
	setAttr ".pt[118]" -type "float3" -0.15385675 0.18861121 -0.0066329222 ;
	setAttr ".pt[119]" -type "float3" 0.16883813 0.18861121 0.047682326 ;
	setAttr ".pt[120]" -type "float3" 0.16883816 -0.16984309 -0.059142709 ;
	setAttr ".pt[121]" -type "float3" -0.13358985 -0.15324636 -0.10886385 ;
	setAttr ".pt[122]" -type "float3" -0.1376418 -0.15432648 0.013694996 ;
	setAttr ".pt[123]" -type "float3" 0.1532913 -0.12192851 0.082651049 ;
	setAttr ".pt[124]" -type "float3" -0.11597655 0.058533344 -0.13331933 ;
	setAttr ".pt[125]" -type "float3" 0.15329133 0.10883079 -0.093857437 ;
	setAttr ".pt[126]" -type "float3" -0.17374878 0.038274217 -0.19897626 ;
	setAttr ".pt[127]" -type "float3" 0.15900101 -0.01938252 -0.19897635 ;
	setAttr ".pt[128]" -type "float3" 0.14762776 -0.11191267 0.17071517 ;
	setAttr ".pt[129]" -type "float3" -0.17374875 -0.059362195 0.19370595 ;
	setAttr ".pt[130]" -type "float3" -0.11656548 -0.064589471 0.124925 ;
	setAttr ".pt[131]" -type "float3" 0.11221991 0.067524232 0.10545548 ;
	setAttr ".pt[132]" -type "float3" -0.026416831 -0.12312317 0.00332422 ;
	setAttr ".pt[133]" -type "float3" 0.1345039 -0.048927147 -0.09326636 ;
	setAttr ".pt[134]" -type "float3" -0.016423222 0.14470176 0.15912588 ;
	setAttr ".pt[135]" -type "float3" 0.13131541 0.13125989 -0.058737021 ;
	setAttr ".pt[136]" -type "float3" -0.068123937 -0.15904129 0.13650681 ;
	setAttr ".pt[137]" -type "float3" 0.088771142 -0.15904129 -0.10128126 ;
	setAttr ".pt[138]" -type "float3" 0.095763087 0.14493845 -0.19440903 ;
	setAttr ".pt[139]" -type "float3" -0.083342172 -0.12181285 -0.19440906 ;
	setAttr ".pt[140]" -type "float3" 0.14683616 0.08070942 0.16636027 ;
	setAttr ".pt[141]" -type "float3" -0.01550355 -0.18757759 0.19028407 ;
	setAttr ".pt[142]" -type "float3" 0.045167401 -0.17197153 -0.18834808 ;
	setAttr ".pt[143]" -type "float3" -0.031507377 0.18887502 -0.18834811 ;
	setAttr ".pt[144]" -type "float3" 0.046772316 0.18887502 0.18404661 ;
	setAttr ".pt[145]" -type "float3" 0.1053008 -0.14439222 0.1542325 ;
	setAttr ".pt[146]" -type "float3" -0.11491008 0.045835331 0.018417113 ;
	setAttr ".pt[147]" -type "float3" -0.051114995 0.12205023 -0.10801338 ;
	setAttr ".pt[148]" -type "float3" 0.10421705 -0.11472619 -0.082666941 ;
	setAttr ".pt[149]" -type "float3" -0.037340723 -0.12979597 0.11500775 ;
	setAttr ".pt[150]" -type "float3" -0.146942 0.080802552 -0.15044659 ;
	setAttr ".pt[151]" -type "float3" 0.015551989 -0.18773918 -0.17439297 ;
	setAttr ".pt[152]" -type "float3" 0.083464414 -0.12190314 0.2043325 ;
	setAttr ".pt[153]" -type "float3" -0.095814832 0.14510731 0.20433246 ;
	setAttr ".pt[154]" -type "float3" -0.13171642 -0.054485459 -0.14092536 ;
	setAttr ".pt[155]" -type "float3" 0.0082910992 0.15063591 -0.15479152 ;
	setAttr ".pt[156]" -type "float3" -0.084914558 -0.10634479 0.18384033 ;
	setAttr ".pt[157]" -type "float3" 0.068757027 0.12289985 0.18384027 ;
	setAttr ".pt[158]" -type "float3" -0.04536679 -0.17632063 -0.19283633 ;
	setAttr ".pt[159]" -type "float3" 0.033156294 0.1932241 -0.1928363 ;
	setAttr ".pt[160]" -type "float3" -0.10694966 -0.14807661 0.15800191 ;
	setAttr ".pt[161]" -type "float3" -0.047010504 0.1932241 0.18853481 ;
	setAttr -s 162 ".vt[0:161]"  -0.54839152 -0.57053566 0.54839152 0.54839152 -0.57053566 0.54839152
		 -0.54839152 0.5262475 0.54839134 0.54839152 0.5262475 0.54839134 -0.54839152 0.5262475 -0.54839152
		 0.54839152 0.52624726 -0.54839152 -0.54839152 -0.57053566 -0.54839152 0.54839152 -0.57053566 -0.54839152
		 -0.69265521 0.66241121 -4.3767642e-08 0.69265509 -0.022144079 -0.69265509 2.5072142e-09 -0.72788918 -0.69265509
		 -0.69265521 -0.022144079 -0.69265509 0.69265509 -0.71479917 -3.3237367e-08 -0.69265521 -0.71479917 9.385289e-09
		 0 -0.71479917 0.69265509 0.69265509 -0.022144079 0.69265509 2.5072142e-09 0.67051125 0.69265509
		 -0.69265509 -0.022144079 0.69265509 0.69265509 0.66241121 -1.1450068e-09 2.5072142e-09 0.67051101 -0.69265509
		 1.805194e-08 -0.022144079 0.97037768 1.7049064e-08 0.94003034 -3.0228723e-08 1.7049064e-08 -0.022144079 -0.98328102
		 1.7049064e-08 -0.98431855 -4.1536801e-09 0.95947438 -0.022144079 -1.6188292e-08 -0.95947438 -0.022144079 -1.8194079e-08
		 -0.3514688 -0.373613 0.82258654 0.3514688 -0.373613 0.82258654 0.35392401 0.37792349 0.80007881
		 -0.35392419 0.37792349 0.80007881 -0.35430562 0.77443838 0.40085706 0.35430562 0.77443838 0.40085706
		 0.35430351 0.77445626 -0.4075692 -0.35430381 0.77445626 -0.40756908 -0.35392419 0.37792325 -0.80007881
		 0.35392419 0.37792349 -0.80007881 0.3514688 -0.40997577 -0.82258654 -0.35146892 -0.40997553 -0.82258654
		 -0.35430369 -0.81874394 -0.40756896 0.35430369 -0.81874394 -0.40756908 0.35430545 -0.8187263 0.40085718
		 -0.35430574 -0.8187263 0.40085706 0.81053001 -0.36250198 0.40085718 0.81053829 -0.36250961 -0.40756908
		 0.80043256 0.36528492 -0.39825597 0.80042452 0.36527634 0.39154413 -0.81053829 -0.36250961 -0.4075692
		 -0.81053001 -0.36250198 0.40085706 -0.80042452 0.36527634 0.39154413 -0.80043256 0.36528492 -0.39825585
		 -1.6046169e-08 -0.399786 0.882411 0.37764195 -0.022144079 0.882411 3.2092338e-08 0.39734626 0.85826617
		 -0.37764195 -0.022144079 0.88241094 -1.6046169e-08 0.83237147 0.42620289 0.37764195 0.8791666 -1.1450068e-09
		 3.2092338e-08 0.83239055 -0.43291539 -0.37764195 0.87916636 -4.9283511e-08 -1.6046169e-08 0.39734626 -0.85826629
		 0.37764195 -0.022144079 -0.91971713 3.2092338e-08 -0.44911492 -0.90589052 -0.37764195 -0.022144079 -0.91971713
		 -1.6046169e-08 -0.87667865 -0.43291539 0.36684224 -0.92075461 -3.3237367e-08 3.2092338e-08 -0.87665951 0.42620289
		 -0.36684224 -0.92075449 1.4901161e-08 0.882411 -0.43218517 -1.1450068e-09 0.86847281 -0.022144079 -0.43291539
		 0.87033856 0.39734626 -4.9565919e-08 0.86846322 -0.022144079 0.42620289 -0.882411 -0.43218517 -3.3237367e-08
		 -0.86846328 -0.022144079 0.42620289 -0.87033856 0.39734626 1.2862518e-08 -0.8684727 -0.022144079 -0.43291539
		 -0.3252956 -0.6671263 0.64498234 0.3252956 -0.6671263 0.64498234 0.64498234 -0.34743977 0.64498234
		 0.63940656 0.35850072 0.62733418 0.3252956 0.6228385 0.64498234 -0.3252956 0.6228385 0.64498234
		 -0.63940656 0.35850072 0.62733418 -0.64498234 -0.34743977 0.64498234 0.63854039 0.6024487 0.37551135
		 0.6385448 0.60246277 -0.38222271 0.3252956 0.6228385 -0.64498228 -0.3252956 0.6228385 -0.64498228
		 -0.6385448 0.60246277 -0.38222274 -0.63854039 0.6024487 0.37551135 0.63940656 0.35850072 -0.62733412
		 0.64498234 -0.34743977 -0.64498228 0.3252956 -0.6671263 -0.64498228 -0.3252956 -0.6671263 -0.64498228
		 -0.64498234 -0.34743977 -0.64498228 -0.63940656 0.35850072 -0.62733412 0.6385448 -0.64675057 -0.38222274
		 0.63854039 -0.64673662 0.37551126 -0.63854039 -0.64673662 0.37551135 -0.6385448 -0.64675057 -0.38222274
		 -0.38717416 0.9006741 0.81907052 -0.061878562 0.94834709 0.86674327 -0.061878577 1.11020708 0.60029107
		 -0.41618419 1.052274227 0.57494521 0.81767005 0.87366009 0.45091328 0.53343529 1.045649767 0.47625896
		 0.87178475 0.93362236 0.07540188 0.55677164 1.15037799 0.07540188 0.80280846 0.83191776 -0.56025702
		 0.51856714 1.0039112568 -0.58560348 0.71265519 0.7557025 -0.72642583 0.48955923 0.85229349 -0.82301658
		 -0.05457985 1.25880361 -0.082054354 -0.4322218 1.19793963 -0.082054384 -0.054579835 1.15116382 -0.51496977
		 -0.40888369 1.093229294 -0.48962346 -0.55879021 0.059236169 -1.18784201 -0.87380344 0.059236169 -0.96077996
		 -0.82055485 0.43988109 -0.895459 -0.53507245 0.45930362 -1.068203688 0.43258962 0.073453546 -1.23463023
		 0.054947693 0.073453546 -1.29819405 0.054947659 0.492944 -1.17317927 0.40887186 0.47352123 -1.11499178
		 0.43037403 -0.60350871 -1.082677484 0.07890524 -0.64264786 -1.16598153 0.40420079 -0.86065918 -0.90507329
		 0.07890521 -0.92142224 -0.95274609 -0.056546241 -1.30317593 -0.080379359 -0.42338851 -1.23961186 -0.080379337
		 -0.4108499 -1.13760138 -0.4879483 -0.056546271 -1.19553602 -0.51329476 0.80328941 -0.87896764 0.54945064
		 0.51905447 -1.050957203 0.57479656 0.71314055 -0.80276668 0.72233093 0.49004456 -0.89935732 0.81892174
		 1.082023859 -0.42568982 0.58423334 0.91647607 -0.4106276 0.82835847 1.13995683 -0.085332036 0.60957903
		 0.96414882 -0.085332036 0.87603122 0.96531618 -0.89213043 -0.074416533 1.15507209 -0.6095165 -0.074416496
		 0.91120595 -0.8240819 -0.45663923 1.083199382 -0.53984094 -0.4819856 1.19111061 0.46260333 -0.06461592
		 1.28024638 0.043112993 -0.064615875 1.18924487 0.043112993 -0.49753127 1.1212045 0.43054199 -0.46287182
		 -0.77571726 -0.73440874 -0.72949082 -0.86587054 -0.81062365 -0.56332201 -1.037864089 -0.5263828 -0.58866847
		 -0.87230808 -0.51131296 -0.82608163 -0.91093636 -0.82395387 0.45116058 -1.082926035 -0.53971922 0.47650626
		 -1.15480709 -0.60940254 0.07564918 -0.96505123 -0.89201641 0.075649224 -0.90854192 0.78710055 0.44132385
		 -1.070426106 0.54992819 0.45735663 -0.96265674 0.84706306 0.065812454 -1.14034009 0.58199811 0.065812506
		 -1.19111061 0.46260333 -0.064615957 -1.2802465 0.043112755 -0.064615987 -1.1212045 0.43054199 -0.46287182
		 -1.18924475 0.043112755 -0.49753138;
	setAttr -s 320 ".ed";
	setAttr ".ed[0:165]"  0 74 1 74 14 1 14 75 1 75 1 0 2 79 1 79 16 0 16 78 1
		 78 3 1 4 85 1 85 19 1 19 84 1 84 5 0 6 91 1 91 10 1 10 90 0 90 7 1 0 81 1 81 17 1
		 17 80 1 80 2 1 1 76 1 77 3 1 2 87 1 87 8 0 8 86 1 86 4 1 3 82 1 83 5 0 4 93 1 93 11 0
		 11 92 1 92 6 0 5 88 1 88 9 1 9 89 1 89 7 1 6 97 0 97 13 1 13 96 0 96 0 1 7 94 1 94 12 0
		 12 95 1 95 1 0 74 26 1 26 81 1 14 50 1 50 26 1 20 53 1 53 26 1 53 17 1 75 27 1 27 50 1
		 76 27 1 15 51 1 51 27 1 51 20 1 28 52 1 52 20 1 77 28 1 78 28 1 16 52 1 29 80 1 52 29 1
		 79 29 1 79 30 0 30 87 1 16 54 0 54 30 0 21 57 0 57 30 1 57 8 1 78 31 1 31 54 1 82 31 0
		 18 55 0 55 31 0 55 21 1 32 56 1 56 21 0 83 32 0 84 32 0 19 56 1 33 86 1 56 33 0 85 33 1
		 85 34 1 34 93 0 19 58 1 58 34 1 58 22 0 22 61 1 61 11 0 84 35 1 35 58 0 88 35 1 9 59 1
		 59 22 0 59 36 1 36 60 0 89 36 1 90 36 0 10 60 0 61 37 1 37 92 1 60 37 1 91 37 1 91 38 1
		 38 97 1 10 62 1 62 38 0 62 23 0 23 65 0 65 13 1 90 39 1 39 62 1 94 39 1 12 63 1 63 23 1
		 63 40 1 40 64 1 95 40 0 75 40 0 14 64 1 65 41 1 41 96 1 64 41 1 74 41 1 95 42 1 42 76 0
		 12 66 0 66 42 1 66 24 1 24 69 1 69 15 0 94 43 0 43 66 0 89 43 1 9 67 1 67 43 1 67 24 0
		 44 68 0 88 44 1 83 44 1 18 68 1 69 45 1 45 77 1 68 45 1 82 45 1 97 46 0 46 92 0 13 70 0
		 70 25 1 73 11 1 96 47 0 81 47 1 17 71 1 71 47 1 80 48 1 87 48 0 8 72 0 73 49 0 49 93 1
		 86 49 1 73 46 1 60 22 1;
	setAttr ".ed[166:319]" 69 42 0 76 15 0 50 20 1 71 48 1 72 25 0 61 34 0 59 35 0
		 67 44 0 68 24 0 15 77 1 51 28 1 53 29 1 48 72 0 71 25 1 47 70 0 64 23 1 82 18 0 54 21 1
		 72 49 0 25 73 0 70 46 1 65 38 0 63 39 1 18 83 1 55 32 1 57 33 0 79 98 0 16 99 0 98 99 0
		 54 100 0 99 100 0 30 101 0 100 101 0 98 101 0 82 102 0 31 103 0 102 103 0 18 104 0
		 102 104 0 55 105 0 104 105 0 105 103 0 83 106 0 32 107 0 106 107 0 5 108 0 106 108 0
		 84 109 0 109 108 0 109 107 0 21 110 0 57 111 0 110 111 0 56 112 0 112 110 0 33 113 0
		 112 113 0 111 113 0 61 114 0 11 115 0 114 115 0 93 116 0 116 115 0 34 117 0 117 116 0
		 114 117 0 59 118 0 22 119 0 118 119 0 58 120 0 120 119 0 35 121 0 121 120 0 118 121 0
		 36 122 0 60 123 0 122 123 0 90 124 0 124 122 0 10 125 0 125 124 0 125 123 0 23 126 0
		 65 127 0 126 127 0 38 128 0 127 128 0 62 129 0 129 128 0 129 126 0 95 130 0 40 131 0
		 130 131 0 1 132 0 130 132 0 75 133 0 133 132 0 133 131 0 42 134 0 76 135 0 134 135 0
		 69 136 0 136 134 0 15 137 0 136 137 0 135 137 0 12 138 0 66 139 0 138 139 0 94 140 0
		 140 138 0 43 141 0 140 141 0 141 139 0 68 142 0 24 143 0 142 143 0 67 144 0 144 143 0
		 44 145 0 144 145 0 145 142 0 6 146 0 97 147 0 146 147 0 46 148 0 147 148 0 92 149 0
		 148 149 0 149 146 0 96 150 0 47 151 0 150 151 0 70 152 0 151 152 0 13 153 0 153 152 0
		 153 150 0 87 154 0 48 155 0 154 155 0 8 156 0 154 156 0 72 157 0 156 157 0 155 157 0
		 72 158 0 25 159 0 158 159 0 49 160 0 158 160 0 73 161 0 161 160 0 159 161 0;
	setAttr -s 160 -ch 640 ".fc[0:159]" -type "polyFaces" 
		f 4 0 44 45 -17
		mu 0 4 0 87 39 96
		f 4 1 46 47 -45
		mu 0 4 87 20 63 39
		f 4 -48 168 48 49
		mu 0 4 39 63 14 66
		f 4 -18 -46 -50 50
		mu 0 4 24 96 39 66
		f 4 2 51 52 -47
		mu 0 4 20 89 40 63
		f 4 3 20 53 -52
		mu 0 4 89 2 91 40
		f 4 -54 167 54 55
		mu 0 4 40 91 22 64
		f 4 -53 -56 56 -169
		mu 0 4 63 40 64 14
		f 4 -57 176 57 58
		mu 0 4 14 64 41 65
		f 4 -55 175 59 -177
		mu 0 4 64 22 92 41
		f 4 -60 21 -8 60
		mu 0 4 41 92 5 93
		f 4 -58 -61 -7 61
		mu 0 4 65 41 93 23
		f 4 -19 -51 177 62
		mu 0 4 95 24 66 42
		f 4 -49 -59 63 -178
		mu 0 4 66 14 65 42
		f 4 -64 -62 -6 64
		mu 0 4 42 65 23 94
		f 4 -63 -65 -5 -20
		mu 0 4 95 42 94 4
		f 4 4 65 66 -23
		mu 0 4 4 94 43 105
		f 4 194 196 198 -200
		mu 0 4 125 126 127 128
		f 4 -69 183 69 70
		mu 0 4 43 67 15 70
		f 4 -24 -67 -71 71
		mu 0 4 28 105 43 70
		f 4 6 72 73 -68
		mu 0 4 23 93 44 67
		f 4 7 26 74 -73
		mu 0 4 93 5 97 44
		f 4 -203 204 206 207
		mu 0 4 129 130 131 132
		f 4 -74 -77 77 -184
		mu 0 4 67 44 68 15
		f 4 -78 190 78 79
		mu 0 4 15 68 45 69
		f 4 -76 189 80 -191
		mu 0 4 68 25 99 45
		f 4 -211 212 -215 215
		mu 0 4 133 134 135 136
		f 4 -79 -82 -11 82
		mu 0 4 69 45 101 27
		f 4 -25 -72 191 83
		mu 0 4 103 28 70 46
		f 4 -219 -221 222 -224
		mu 0 4 137 138 139 140
		f 4 -85 -83 -10 85
		mu 0 4 46 69 27 102
		f 4 -84 -86 -9 -26
		mu 0 4 103 46 102 6
		f 4 8 86 87 -29
		mu 0 4 6 102 47 115
		f 4 9 88 89 -87
		mu 0 4 102 27 71 47
		f 4 91 171 -90 90
		mu 0 4 16 74 47 71
		f 4 226 -229 -231 -232
		mu 0 4 141 142 143 144
		f 4 10 93 94 -89
		mu 0 4 27 101 48 71
		f 4 11 32 95 -94
		mu 0 4 101 8 107 48
		f 4 96 172 -96 33
		mu 0 4 30 72 48 107
		f 4 234 -237 -239 -240
		mu 0 4 145 146 147 148
		f 4 99 165 -98 98
		mu 0 4 49 73 16 72
		f 4 100 -99 -97 34
		mu 0 4 109 49 72 30
		f 4 -101 35 -16 101
		mu 0 4 49 109 12 111
		f 4 -243 -245 -247 247
		mu 0 4 149 150 151 152
		f 4 104 -31 -93 103
		mu 0 4 50 113 33 74
		f 4 105 -104 -92 -166
		mu 0 4 73 50 74 16
		f 4 -106 -103 -14 106
		mu 0 4 50 73 32 112
		f 4 -105 -107 -13 -32
		mu 0 4 113 50 112 10
		f 4 12 107 108 -37
		mu 0 4 10 112 51 123
		f 4 13 109 110 -108
		mu 0 4 112 32 75 51
		f 4 250 252 -255 255
		mu 0 4 153 154 155 156
		f 4 113 -38 -109 -188
		mu 0 4 78 37 123 51
		f 4 14 114 115 -110
		mu 0 4 32 111 52 75
		f 4 15 40 116 -115
		mu 0 4 111 12 117 52
		f 4 117 188 -117 41
		mu 0 4 35 76 52 117
		f 4 118 -112 -116 -189
		mu 0 4 76 17 75 52
		f 4 120 181 -119 119
		mu 0 4 53 77 17 76
		f 4 121 -120 -118 42
		mu 0 4 119 53 76 35
		f 4 -259 260 -263 263
		mu 0 4 157 158 159 160
		f 4 -121 -123 -3 123
		mu 0 4 77 53 90 21
		f 4 125 -39 -114 124
		mu 0 4 54 121 37 78
		f 4 126 -125 -113 -182
		mu 0 4 77 54 78 17
		f 4 -127 -124 -2 127
		mu 0 4 54 77 21 88
		f 4 -126 -128 -1 -40
		mu 0 4 121 54 88 1
		f 4 -44 128 129 -21
		mu 0 4 2 120 55 91
		f 4 -129 -43 130 131
		mu 0 4 55 120 36 79
		f 4 166 -132 132 133
		mu 0 4 82 55 79 18
		f 4 -267 -269 270 -272
		mu 0 4 161 162 163 164
		f 4 -275 -277 278 279
		mu 0 4 165 166 167 168
		f 4 -41 -36 137 -136
		mu 0 4 118 13 110 56
		f 4 -138 -35 138 139
		mu 0 4 56 110 31 80
		f 4 -133 -137 -140 140
		mu 0 4 18 79 56 80
		f 4 282 -285 286 287
		mu 0 4 169 170 171 172
		f 4 -139 -34 142 -174
		mu 0 4 80 31 108 57
		f 4 -143 -33 -28 143
		mu 0 4 57 108 9 100
		f 4 144 -142 -144 -190
		mu 0 4 26 81 57 100
		f 4 -135 145 146 -176
		mu 0 4 22 82 58 92
		f 4 -146 -134 -175 147
		mu 0 4 58 82 18 81
		f 4 148 -148 -145 -183
		mu 0 4 98 58 81 26
		f 4 -147 -149 -27 -22
		mu 0 4 92 58 98 5
		f 4 290 292 294 295
		mu 0 4 173 174 175 176
		f 4 151 186 -150 37
		mu 0 4 38 83 59 124
		f 4 152 185 164 -187
		mu 0 4 83 19 86 59
		f 4 -151 -165 153 30
		mu 0 4 114 59 86 34
		f 4 298 300 -303 303
		mu 0 4 177 178 179 180
		f 4 39 16 155 -155
		mu 0 4 122 0 96 60
		f 4 -156 17 156 157
		mu 0 4 60 96 24 84
		f 4 -158 179 -153 -181
		mu 0 4 60 84 19 83
		f 4 169 178 170 -180
		mu 0 4 84 61 85 19
		f 4 -157 18 158 -170
		mu 0 4 84 24 95 61
		f 4 -159 19 22 159
		mu 0 4 61 95 4 106
		f 4 -307 308 310 -312
		mu 0 4 181 182 183 184
		f 4 -154 161 162 29
		mu 0 4 34 86 62 116
		f 4 -315 316 -319 -320
		mu 0 4 185 188 186 187
		f 4 -161 24 163 -185
		mu 0 4 85 29 104 62
		f 4 -163 -164 25 28
		mu 0 4 116 62 104 7
		f 4 5 193 -195 -193
		mu 0 4 94 23 126 125
		f 4 67 195 -197 -194
		mu 0 4 23 67 127 126
		f 4 68 197 -199 -196
		mu 0 4 67 43 128 127
		f 4 -66 192 199 -198
		mu 0 4 43 94 125 128
		f 4 -75 200 202 -202
		mu 0 4 44 97 130 129
		f 4 182 203 -205 -201
		mu 0 4 97 25 131 130
		f 4 75 205 -207 -204
		mu 0 4 25 68 132 131
		f 4 76 201 -208 -206
		mu 0 4 68 44 129 132
		f 4 -81 208 210 -210
		mu 0 4 45 99 134 133
		f 4 27 211 -213 -209
		mu 0 4 99 8 135 134
		f 4 -12 213 214 -212
		mu 0 4 8 101 136 135
		f 4 81 209 -216 -214
		mu 0 4 101 45 133 136
		f 4 -70 216 218 -218
		mu 0 4 70 15 138 137
		f 4 -80 219 220 -217
		mu 0 4 15 69 139 138
		f 4 84 221 -223 -220
		mu 0 4 69 46 140 139
		f 4 -192 217 223 -222
		mu 0 4 46 70 137 140
		f 4 92 225 -227 -225
		mu 0 4 74 33 142 141
		f 4 -30 227 228 -226
		mu 0 4 33 115 143 142
		f 4 -88 229 230 -228
		mu 0 4 115 47 144 143
		f 4 -172 224 231 -230
		mu 0 4 47 74 141 144
		f 4 97 233 -235 -233
		mu 0 4 72 16 146 145
		f 4 -91 235 236 -234
		mu 0 4 16 71 147 146
		f 4 -95 237 238 -236
		mu 0 4 71 48 148 147
		f 4 -173 232 239 -238
		mu 0 4 48 72 145 148
		f 4 -100 240 242 -242
		mu 0 4 73 49 150 149
		f 4 -102 243 244 -241
		mu 0 4 49 111 151 150
		f 4 -15 245 246 -244
		mu 0 4 111 32 152 151
		f 4 102 241 -248 -246
		mu 0 4 32 73 149 152
		f 4 112 249 -251 -249
		mu 0 4 17 78 154 153
		f 4 187 251 -253 -250
		mu 0 4 78 51 155 154
		f 4 -111 253 254 -252
		mu 0 4 51 75 156 155
		f 4 111 248 -256 -254
		mu 0 4 75 17 153 156
		f 4 -122 256 258 -258
		mu 0 4 53 119 158 157
		f 4 43 259 -261 -257
		mu 0 4 119 3 159 158
		f 4 -4 261 262 -260
		mu 0 4 3 90 160 159
		f 4 122 257 -264 -262
		mu 0 4 90 53 157 160
		f 4 -130 264 266 -266
		mu 0 4 91 55 162 161
		f 4 -167 267 268 -265
		mu 0 4 55 82 163 162
		f 4 134 269 -271 -268
		mu 0 4 82 22 164 163
		f 4 -168 265 271 -270
		mu 0 4 22 91 161 164
		f 4 -131 272 274 -274
		mu 0 4 79 36 166 165
		f 4 -42 275 276 -273
		mu 0 4 36 118 167 166
		f 4 135 277 -279 -276
		mu 0 4 118 56 168 167
		f 4 136 273 -280 -278
		mu 0 4 56 79 165 168
		f 4 174 281 -283 -281
		mu 0 4 81 18 170 169
		f 4 -141 283 284 -282
		mu 0 4 18 80 171 170
		f 4 173 285 -287 -284
		mu 0 4 80 57 172 171
		f 4 141 280 -288 -286
		mu 0 4 57 81 169 172
		f 4 36 289 -291 -289
		mu 0 4 11 124 174 173
		f 4 149 291 -293 -290
		mu 0 4 124 59 175 174
		f 4 150 293 -295 -292
		mu 0 4 59 114 176 175
		f 4 31 288 -296 -294
		mu 0 4 114 11 173 176
		f 4 154 297 -299 -297
		mu 0 4 122 60 178 177
		f 4 180 299 -301 -298
		mu 0 4 60 83 179 178
		f 4 -152 301 302 -300
		mu 0 4 83 38 180 179
		f 4 38 296 -304 -302
		mu 0 4 38 122 177 180
		f 4 -160 304 306 -306
		mu 0 4 61 106 182 181
		f 4 23 307 -309 -305
		mu 0 4 106 29 183 182
		f 4 160 309 -311 -308
		mu 0 4 29 85 184 183
		f 4 -179 305 311 -310
		mu 0 4 85 61 181 184
		f 4 -171 312 314 -314
		mu 0 4 19 85 188 185
		f 4 184 315 -317 -313
		mu 0 4 85 62 186 188
		f 4 -162 317 318 -316
		mu 0 4 62 86 187 186
		f 4 -186 313 319 -318
		mu 0 4 86 19 185 187;
	setAttr ".cd" -type "dataPolyComponent" Index_Data Edge 0 ;
	setAttr ".cvd" -type "dataPolyComponent" Index_Data Vertex 0 ;
	setAttr ".pd[0]" -type "dataPolyComponent" Index_Data UV 0 ;
	setAttr ".hfd" -type "dataPolyComponent" Index_Data Face 0 ;
createNode transform -n "pCube3";
	rename -uid "E3D94E5F-49D8-71DE-DED5-9EBD910938B6";
	setAttr ".t" -type "double3" 0 3.679833024183659 0 ;
	setAttr ".s" -type "double3" 2.3222049723809342 2.3222049723809342 2.3222049723809342 ;
	setAttr ".spt" -type "double3" 0 6.3177268003519534e-17 -1.9702389424755185e-08 ;
createNode mesh -n "pCubeShape3" -p "pCube3";
	rename -uid "1D40996A-46EA-49DE-8B8C-4C9675072A87";
	setAttr -k off ".v";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr ".pv" -type "double2" 0.68263136513833622 0.33428172961525293 ;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr -s 6 ".pt";
	setAttr ".pt[206]" -type "float3" 0 1.4901161e-08 0 ;
	setAttr ".pt[207]" -type "float3" 0 1.4901161e-08 0 ;
	setAttr ".pt[208]" -type "float3" 0 1.4901161e-08 0 ;
	setAttr ".pt[209]" -type "float3" 0 1.4901161e-08 0 ;
createNode lightLinker -s -n "lightLinker1";
	rename -uid "B690C022-43BD-B192-559E-2582BEFEF69B";
	setAttr -s 2 ".lnk";
	setAttr -s 2 ".slnk";
createNode shapeEditorManager -n "shapeEditorManager";
	rename -uid "06C3D0F1-4C4E-822C-C1FC-6785AAD82766";
createNode poseInterpolatorManager -n "poseInterpolatorManager";
	rename -uid "9D12CB9F-4F5B-179F-15B4-1CB4FF988438";
createNode displayLayerManager -n "layerManager";
	rename -uid "60693A9C-498D-97C8-4604-0D980CF52F46";
createNode displayLayer -n "defaultLayer";
	rename -uid "45ADB46C-4F25-F579-6787-319411CBC99C";
	setAttr ".ufem" -type "stringArray" 0  ;
createNode renderLayerManager -n "renderLayerManager";
	rename -uid "27D3AF19-41E2-ECEC-9BEA-3DB59B1A7B91";
createNode renderLayer -n "defaultRenderLayer";
	rename -uid "43B5CFF9-4131-31DD-9FC2-539CDB472FD3";
	setAttr ".g" yes;
createNode aiOptions -s -n "defaultArnoldRenderOptions";
	rename -uid "FD8EE62D-472C-1A71-E994-E49C14D9A88C";
	setAttr ".version" -type "string" "5.3.0";
createNode aiAOVFilter -s -n "defaultArnoldFilter";
	rename -uid "657D5ABF-431F-0B0C-B206-21A5153DDD20";
	setAttr ".ai_translator" -type "string" "gaussian";
createNode aiAOVDriver -s -n "defaultArnoldDriver";
	rename -uid "E281AE0E-4759-D968-C6BC-6AA4D8D9F99E";
	setAttr ".ai_translator" -type "string" "exr";
createNode aiAOVDriver -s -n "defaultArnoldDisplayDriver";
	rename -uid "8676CA1F-4F66-83E1-829A-AE8C9EE688C5";
	setAttr ".output_mode" 0;
	setAttr ".ai_translator" -type "string" "maya";
createNode script -n "uiConfigurationScriptNode";
	rename -uid "59F582E3-4BAB-28A6-7ADA-1D9845FC7908";
	setAttr ".b" -type "string" (
		"// Maya Mel UI Configuration File.\n//\n//  This script is machine generated.  Edit at your own risk.\n//\n//\n\nglobal string $gMainPane;\nif (`paneLayout -exists $gMainPane`) {\n\n\tglobal int $gUseScenePanelConfig;\n\tint    $useSceneConfig = $gUseScenePanelConfig;\n\tint    $nodeEditorPanelVisible = stringArrayContains(\"nodeEditorPanel1\", `getPanel -vis`);\n\tint    $nodeEditorWorkspaceControlOpen = (`workspaceControl -exists nodeEditorPanel1Window` && `workspaceControl -q -visible nodeEditorPanel1Window`);\n\tint    $menusOkayInPanels = `optionVar -q allowMenusInPanels`;\n\tint    $nVisPanes = `paneLayout -q -nvp $gMainPane`;\n\tint    $nPanes = 0;\n\tstring $editorName;\n\tstring $panelName;\n\tstring $itemFilterName;\n\tstring $panelConfig;\n\n\t//\n\t//  get current state of the UI\n\t//\n\tsceneUIReplacement -update $gMainPane;\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Top View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Top View\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|top\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 0\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n"
		+ "            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n"
		+ "            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -bluePencil 1\n            -greasePencils 0\n            -excludeObjectPreset \"All\" \n            -shadows 0\n            -captureSequenceNumber -1\n            -width 821\n            -height 517\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n"
		+ "\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Side View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Side View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|side\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 0\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n"
		+ "            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n"
		+ "            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -bluePencil 1\n            -greasePencils 0\n            -excludeObjectPreset \"All\" \n"
		+ "            -shadows 0\n            -captureSequenceNumber -1\n            -width 820\n            -height 517\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Front View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Front View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|front\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n"
		+ "            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 0\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n"
		+ "            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n"
		+ "            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -bluePencil 1\n            -greasePencils 0\n            -excludeObjectPreset \"All\" \n            -shadows 0\n            -captureSequenceNumber -1\n            -width 944\n            -height 647\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Persp View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Persp View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n"
		+ "        modelEditor -e \n            -camera \"|persp\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 1\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 0\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n"
		+ "            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n"
		+ "            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -bluePencil 1\n            -greasePencils 0\n            -excludeObjectPreset \"All\" \n            -shadows 0\n            -captureSequenceNumber -1\n            -width 943\n            -height 647\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n"
		+ "\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"outlinerPanel\" (localizedPanelLabel(\"ToggledOutliner\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\toutlinerPanel -edit -l (localizedPanelLabel(\"ToggledOutliner\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        outlinerEditor -e \n            -showShapes 0\n            -showAssignedMaterials 0\n            -showTimeEditor 1\n            -showReferenceNodes 1\n            -showReferenceMembers 1\n            -showAttributes 0\n            -showConnected 0\n            -showAnimCurvesOnly 0\n            -showMuteInfo 0\n            -organizeByLayer 1\n            -organizeByClip 1\n            -showAnimLayerWeight 1\n            -autoExpandLayers 1\n            -autoExpand 0\n            -showDagOnly 1\n            -showAssets 1\n            -showContainedOnly 1\n            -showPublishedAsConnected 0\n            -showParentContainers 0\n            -showContainerContents 1\n            -ignoreDagHierarchy 0\n"
		+ "            -expandConnections 0\n            -showUpstreamCurves 1\n            -showUnitlessCurves 1\n            -showCompounds 1\n            -showLeafs 1\n            -showNumericAttrsOnly 0\n            -highlightActive 1\n            -autoSelectNewObjects 0\n            -doNotSelectNewObjects 0\n            -dropIsParent 1\n            -transmitFilters 0\n            -setFilter \"defaultSetFilter\" \n            -showSetMembers 1\n            -allowMultiSelection 1\n            -alwaysToggleSelect 0\n            -directSelect 0\n            -isSet 0\n            -isSetMember 0\n            -showUfeItems 1\n            -displayMode \"DAG\" \n            -expandObjects 0\n            -setsIgnoreFilters 1\n            -containersIgnoreFilters 0\n            -editAttrName 0\n            -showAttrValues 0\n            -highlightSecondary 0\n            -showUVAttrsOnly 0\n            -showTextureNodesOnly 0\n            -attrAlphaOrder \"default\" \n            -animLayerFilterOptions \"allAffecting\" \n            -sortOrder \"none\" \n            -longNames 0\n"
		+ "            -niceNames 1\n            -showNamespace 1\n            -showPinIcons 0\n            -mapMotionTrails 0\n            -ignoreHiddenAttribute 0\n            -ignoreOutlinerColor 0\n            -renderFilterVisible 0\n            -renderFilterIndex 0\n            -selectionOrder \"chronological\" \n            -expandAttribute 0\n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"outlinerPanel\" (localizedPanelLabel(\"Outliner\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\toutlinerPanel -edit -l (localizedPanelLabel(\"Outliner\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        outlinerEditor -e \n            -showShapes 0\n            -showAssignedMaterials 0\n            -showTimeEditor 1\n            -showReferenceNodes 0\n            -showReferenceMembers 0\n            -showAttributes 0\n            -showConnected 0\n            -showAnimCurvesOnly 0\n            -showMuteInfo 0\n"
		+ "            -organizeByLayer 1\n            -organizeByClip 1\n            -showAnimLayerWeight 1\n            -autoExpandLayers 1\n            -autoExpand 0\n            -showDagOnly 1\n            -showAssets 1\n            -showContainedOnly 1\n            -showPublishedAsConnected 0\n            -showParentContainers 0\n            -showContainerContents 1\n            -ignoreDagHierarchy 0\n            -expandConnections 0\n            -showUpstreamCurves 1\n            -showUnitlessCurves 1\n            -showCompounds 1\n            -showLeafs 1\n            -showNumericAttrsOnly 0\n            -highlightActive 1\n            -autoSelectNewObjects 0\n            -doNotSelectNewObjects 0\n            -dropIsParent 1\n            -transmitFilters 0\n            -setFilter \"defaultSetFilter\" \n            -showSetMembers 1\n            -allowMultiSelection 1\n            -alwaysToggleSelect 0\n            -directSelect 0\n            -showUfeItems 1\n            -displayMode \"DAG\" \n            -expandObjects 0\n            -setsIgnoreFilters 1\n"
		+ "            -containersIgnoreFilters 0\n            -editAttrName 0\n            -showAttrValues 0\n            -highlightSecondary 0\n            -showUVAttrsOnly 0\n            -showTextureNodesOnly 0\n            -attrAlphaOrder \"default\" \n            -animLayerFilterOptions \"allAffecting\" \n            -sortOrder \"none\" \n            -longNames 0\n            -niceNames 1\n            -showNamespace 1\n            -showPinIcons 0\n            -mapMotionTrails 0\n            -ignoreHiddenAttribute 0\n            -ignoreOutlinerColor 0\n            -renderFilterVisible 0\n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"graphEditor\" (localizedPanelLabel(\"Graph Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Graph Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"OutlineEd\");\n            outlinerEditor -e \n                -showShapes 1\n"
		+ "                -showAssignedMaterials 0\n                -showTimeEditor 1\n                -showReferenceNodes 0\n                -showReferenceMembers 0\n                -showAttributes 1\n                -showConnected 1\n                -showAnimCurvesOnly 1\n                -showMuteInfo 0\n                -organizeByLayer 1\n                -organizeByClip 1\n                -showAnimLayerWeight 1\n                -autoExpandLayers 1\n                -autoExpand 1\n                -showDagOnly 0\n                -showAssets 1\n                -showContainedOnly 0\n                -showPublishedAsConnected 0\n                -showParentContainers 0\n                -showContainerContents 0\n                -ignoreDagHierarchy 0\n                -expandConnections 1\n                -showUpstreamCurves 1\n                -showUnitlessCurves 1\n                -showCompounds 0\n                -showLeafs 1\n                -showNumericAttrsOnly 1\n                -highlightActive 0\n                -autoSelectNewObjects 1\n                -doNotSelectNewObjects 0\n"
		+ "                -dropIsParent 1\n                -transmitFilters 1\n                -setFilter \"0\" \n                -showSetMembers 0\n                -allowMultiSelection 1\n                -alwaysToggleSelect 0\n                -directSelect 0\n                -showUfeItems 1\n                -displayMode \"DAG\" \n                -expandObjects 0\n                -setsIgnoreFilters 1\n                -containersIgnoreFilters 0\n                -editAttrName 0\n                -showAttrValues 0\n                -highlightSecondary 0\n                -showUVAttrsOnly 0\n                -showTextureNodesOnly 0\n                -attrAlphaOrder \"default\" \n                -animLayerFilterOptions \"allAffecting\" \n                -sortOrder \"none\" \n                -longNames 0\n                -niceNames 1\n                -showNamespace 1\n                -showPinIcons 1\n                -mapMotionTrails 1\n                -ignoreHiddenAttribute 0\n                -ignoreOutlinerColor 0\n                -renderFilterVisible 0\n                $editorName;\n"
		+ "\n\t\t\t$editorName = ($panelName+\"GraphEd\");\n            animCurveEditor -e \n                -displayValues 0\n                -snapTime \"integer\" \n                -snapValue \"none\" \n                -showPlayRangeShades \"on\" \n                -lockPlayRangeShades \"off\" \n                -smoothness \"fine\" \n                -resultSamples 1\n                -resultScreenSamples 0\n                -resultUpdate \"delayed\" \n                -showUpstreamCurves 1\n                -keyMinScale 1\n                -stackedCurvesMin -1\n                -stackedCurvesMax 1\n                -stackedCurvesSpace 0.2\n                -preSelectionHighlight 0\n                -constrainDrag 0\n                -valueLinesToggle 0\n                -highlightAffectedCurves 0\n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dopeSheetPanel\" (localizedPanelLabel(\"Dope Sheet\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n"
		+ "\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Dope Sheet\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"OutlineEd\");\n            outlinerEditor -e \n                -showShapes 1\n                -showAssignedMaterials 0\n                -showTimeEditor 1\n                -showReferenceNodes 0\n                -showReferenceMembers 0\n                -showAttributes 1\n                -showConnected 1\n                -showAnimCurvesOnly 1\n                -showMuteInfo 0\n                -organizeByLayer 1\n                -organizeByClip 1\n                -showAnimLayerWeight 1\n                -autoExpandLayers 1\n                -autoExpand 0\n                -showDagOnly 0\n                -showAssets 1\n                -showContainedOnly 0\n                -showPublishedAsConnected 0\n                -showParentContainers 0\n                -showContainerContents 0\n                -ignoreDagHierarchy 0\n                -expandConnections 1\n                -showUpstreamCurves 1\n                -showUnitlessCurves 0\n"
		+ "                -showCompounds 1\n                -showLeafs 1\n                -showNumericAttrsOnly 1\n                -highlightActive 0\n                -autoSelectNewObjects 0\n                -doNotSelectNewObjects 1\n                -dropIsParent 1\n                -transmitFilters 0\n                -setFilter \"0\" \n                -showSetMembers 0\n                -allowMultiSelection 1\n                -alwaysToggleSelect 0\n                -directSelect 0\n                -showUfeItems 1\n                -displayMode \"DAG\" \n                -expandObjects 0\n                -setsIgnoreFilters 1\n                -containersIgnoreFilters 0\n                -editAttrName 0\n                -showAttrValues 0\n                -highlightSecondary 0\n                -showUVAttrsOnly 0\n                -showTextureNodesOnly 0\n                -attrAlphaOrder \"default\" \n                -animLayerFilterOptions \"allAffecting\" \n                -sortOrder \"none\" \n                -longNames 0\n                -niceNames 1\n                -showNamespace 1\n"
		+ "                -showPinIcons 0\n                -mapMotionTrails 1\n                -ignoreHiddenAttribute 0\n                -ignoreOutlinerColor 0\n                -renderFilterVisible 0\n                $editorName;\n\n\t\t\t$editorName = ($panelName+\"DopeSheetEd\");\n            dopeSheetEditor -e \n                -displayValues 0\n                -snapTime \"integer\" \n                -snapValue \"none\" \n                -outliner \"dopeSheetPanel1OutlineEd\" \n                -showSummary 1\n                -showScene 0\n                -hierarchyBelow 0\n                -showTicks 1\n                -selectionWindow 0 0 0 0 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"timeEditorPanel\" (localizedPanelLabel(\"Time Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Time Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n"
		+ "\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"clipEditorPanel\" (localizedPanelLabel(\"Trax Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Trax Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = clipEditorNameFromPanel($panelName);\n            clipEditor -e \n                -displayValues 0\n                -snapTime \"none\" \n                -snapValue \"none\" \n                -initialized 0\n                -manageSequencer 0 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"sequenceEditorPanel\" (localizedPanelLabel(\"Camera Sequencer\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Camera Sequencer\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = sequenceEditorNameFromPanel($panelName);\n            clipEditor -e \n                -displayValues 0\n"
		+ "                -snapTime \"none\" \n                -snapValue \"none\" \n                -initialized 0\n                -manageSequencer 1 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"hyperGraphPanel\" (localizedPanelLabel(\"Hypergraph Hierarchy\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Hypergraph Hierarchy\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"HyperGraphEd\");\n            hyperGraph -e \n                -graphLayoutStyle \"hierarchicalLayout\" \n                -orientation \"horiz\" \n                -mergeConnections 0\n                -zoom 1\n                -animateTransition 0\n                -showRelationships 1\n                -showShapes 0\n                -showDeformers 0\n                -showExpressions 0\n                -showConstraints 0\n                -showConnectionFromSelected 0\n                -showConnectionToSelected 0\n"
		+ "                -showConstraintLabels 0\n                -showUnderworld 0\n                -showInvisible 0\n                -transitionFrames 1\n                -opaqueContainers 0\n                -freeform 0\n                -imagePosition 0 0 \n                -imageScale 1\n                -imageEnabled 0\n                -graphType \"DAG\" \n                -heatMapDisplay 0\n                -updateSelection 1\n                -updateNodeAdded 1\n                -useDrawOverrideColor 0\n                -limitGraphTraversal -1\n                -range 0 0 \n                -iconSize \"smallIcons\" \n                -showCachedConnections 0\n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"hyperShadePanel\" (localizedPanelLabel(\"Hypershade\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Hypershade\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n"
		+ "\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"visorPanel\" (localizedPanelLabel(\"Visor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Visor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"nodeEditorPanel\" (localizedPanelLabel(\"Node Editor\")) `;\n\tif ($nodeEditorPanelVisible || $nodeEditorWorkspaceControlOpen) {\n\t\tif (\"\" == $panelName) {\n\t\t\tif ($useSceneConfig) {\n\t\t\t\t$panelName = `scriptedPanel -unParent  -type \"nodeEditorPanel\" -l (localizedPanelLabel(\"Node Editor\")) -mbv $menusOkayInPanels `;\n\n\t\t\t$editorName = ($panelName+\"NodeEditorEd\");\n            nodeEditor -e \n                -allAttributes 0\n                -allNodes 0\n                -autoSizeNodes 1\n                -consistentNameSize 1\n                -createNodeCommand \"nodeEdCreateNodeCommand\" \n                -connectNodeOnCreation 0\n"
		+ "                -connectOnDrop 0\n                -copyConnectionsOnPaste 0\n                -connectionStyle \"bezier\" \n                -defaultPinnedState 0\n                -additiveGraphingMode 0\n                -connectedGraphingMode 1\n                -settingsChangedCallback \"nodeEdSyncControls\" \n                -traversalDepthLimit -1\n                -keyPressCommand \"nodeEdKeyPressCommand\" \n                -nodeTitleMode \"name\" \n                -gridSnap 0\n                -gridVisibility 1\n                -crosshairOnEdgeDragging 0\n                -popupMenuScript \"nodeEdBuildPanelMenus\" \n                -showNamespace 1\n                -showShapes 1\n                -showSGShapes 0\n                -showTransforms 1\n                -useAssets 1\n                -syncedSelection 1\n                -extendToShapes 1\n                -showUnitConversions 0\n                -editorMode \"default\" \n                -hasWatchpoint 0\n                $editorName;\n\t\t\t}\n\t\t} else {\n\t\t\t$label = `panel -q -label $panelName`;\n"
		+ "\t\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Node Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"NodeEditorEd\");\n            nodeEditor -e \n                -allAttributes 0\n                -allNodes 0\n                -autoSizeNodes 1\n                -consistentNameSize 1\n                -createNodeCommand \"nodeEdCreateNodeCommand\" \n                -connectNodeOnCreation 0\n                -connectOnDrop 0\n                -copyConnectionsOnPaste 0\n                -connectionStyle \"bezier\" \n                -defaultPinnedState 0\n                -additiveGraphingMode 0\n                -connectedGraphingMode 1\n                -settingsChangedCallback \"nodeEdSyncControls\" \n                -traversalDepthLimit -1\n                -keyPressCommand \"nodeEdKeyPressCommand\" \n                -nodeTitleMode \"name\" \n                -gridSnap 0\n                -gridVisibility 1\n                -crosshairOnEdgeDragging 0\n                -popupMenuScript \"nodeEdBuildPanelMenus\" \n                -showNamespace 1\n"
		+ "                -showShapes 1\n                -showSGShapes 0\n                -showTransforms 1\n                -useAssets 1\n                -syncedSelection 1\n                -extendToShapes 1\n                -showUnitConversions 0\n                -editorMode \"default\" \n                -hasWatchpoint 0\n                $editorName;\n\t\t\tif (!$useSceneConfig) {\n\t\t\t\tpanel -e -l $label $panelName;\n\t\t\t}\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"createNodePanel\" (localizedPanelLabel(\"Create Node\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Create Node\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"polyTexturePlacementPanel\" (localizedPanelLabel(\"UV Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"UV Editor\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"renderWindowPanel\" (localizedPanelLabel(\"Render View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Render View\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"shapePanel\" (localizedPanelLabel(\"Shape Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tshapePanel -edit -l (localizedPanelLabel(\"Shape Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"posePanel\" (localizedPanelLabel(\"Pose Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tposePanel -edit -l (localizedPanelLabel(\"Pose Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n"
		+ "\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dynRelEdPanel\" (localizedPanelLabel(\"Dynamic Relationships\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Dynamic Relationships\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"relationshipPanel\" (localizedPanelLabel(\"Relationship Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Relationship Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"referenceEditorPanel\" (localizedPanelLabel(\"Reference Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Reference Editor\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dynPaintScriptedPanelType\" (localizedPanelLabel(\"Paint Effects\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Paint Effects\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"scriptEditorPanel\" (localizedPanelLabel(\"Script Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Script Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"profilerPanel\" (localizedPanelLabel(\"Profiler Tool\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Profiler Tool\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"contentBrowserPanel\" (localizedPanelLabel(\"Content Browser\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Content Browser\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|persp\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 1\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n"
		+ "            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 1\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 0\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 4 4 \n            -bumpResolution 4 4 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n"
		+ "            -lowQualityLighting 0\n            -maximumNumHardwareLights 0\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n"
		+ "            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -bluePencil 1\n            -greasePencils 0\n            -shadows 0\n            -captureSequenceNumber -1\n            -width 0\n            -height 0\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\tif ($useSceneConfig) {\n        string $configName = `getPanel -cwl (localizedPanelLabel(\"Current Layout\"))`;\n        if (\"\" != $configName) {\n\t\t\tpanelConfiguration -edit -label (localizedPanelLabel(\"Current Layout\")) \n\t\t\t\t-userCreated false\n\t\t\t\t-defaultImage \"\"\n\t\t\t\t-image \"\"\n\t\t\t\t-sc false\n\t\t\t\t-configString \"global string $gMainPane; paneLayout -e -cn \\\"vertical2\\\" -ps 1 50 100 -ps 2 50 100 $gMainPane;\"\n"
		+ "\t\t\t\t-removeAllPanels\n\t\t\t\t-ap false\n\t\t\t\t\t(localizedPanelLabel(\"Front View\")) \n\t\t\t\t\t\"modelPanel\"\n"
		+ "\t\t\t\t\t\"$panelName = `modelPanel -unParent -l (localizedPanelLabel(\\\"Front View\\\")) -mbv $menusOkayInPanels `;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera front` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 0\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 0\\n    -activeComponentsXray 0\\n    -displayTextures 0\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -bluePencil 1\\n    -greasePencils 0\\n    -excludeObjectPreset \\\"All\\\" \\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 944\\n    -height 647\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t\t\"modelPanel -edit -l (localizedPanelLabel(\\\"Front View\\\")) -mbv $menusOkayInPanels  $panelName;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera front` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 0\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 0\\n    -activeComponentsXray 0\\n    -displayTextures 0\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -bluePencil 1\\n    -greasePencils 0\\n    -excludeObjectPreset \\\"All\\\" \\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 944\\n    -height 647\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t-ap false\n\t\t\t\t\t(localizedPanelLabel(\"Persp View\")) \n\t\t\t\t\t\"modelPanel\"\n"
		+ "\t\t\t\t\t\"$panelName = `modelPanel -unParent -l (localizedPanelLabel(\\\"Persp View\\\")) -mbv $menusOkayInPanels `;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera persp` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 1\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 0\\n    -activeComponentsXray 0\\n    -displayTextures 0\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -bluePencil 1\\n    -greasePencils 0\\n    -excludeObjectPreset \\\"All\\\" \\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 943\\n    -height 647\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t\t\"modelPanel -edit -l (localizedPanelLabel(\\\"Persp View\\\")) -mbv $menusOkayInPanels  $panelName;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera persp` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 1\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 0\\n    -activeComponentsXray 0\\n    -displayTextures 0\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -bluePencil 1\\n    -greasePencils 0\\n    -excludeObjectPreset \\\"All\\\" \\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 943\\n    -height 647\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t$configName;\n\n            setNamedPanelLayout (localizedPanelLabel(\"Current Layout\"));\n        }\n\n        panelHistory -e -clear mainPanelHistory;\n        sceneUIReplacement -clear;\n\t}\n\n\ngrid -spacing 5 -size 12 -divisions 5 -displayAxes yes -displayGridLines yes -displayDivisionLines yes -displayPerspectiveLabels no -displayOrthographicLabels no -displayAxesBold yes -perspectiveLabelPosition axis -orthographicLabelPosition edge;\nviewManip -drawCompass 0 -compassAngle 0 -frontParameters \"\" -homeParameters \"\" -selectionLockParameters \"\";\n}\n");
	setAttr ".st" 3;
createNode script -n "sceneConfigurationScriptNode";
	rename -uid "47324137-42E5-8D68-FEE3-7680CB624D73";
	setAttr ".b" -type "string" "playbackOptions -min 1 -max 120 -ast 1 -aet 200 ";
	setAttr ".st" 6;
createNode nodeGraphEditorInfo -n "hyperShadePrimaryNodeEditorSavedTabsInfo";
	rename -uid "55D42267-4261-5F7D-7F0D-1887BD66C167";
	setAttr ".tgi[0].tn" -type "string" "Untitled_1";
	setAttr ".tgi[0].vl" -type "double2" -44.444442678380966 -429.36506230364472 ;
	setAttr ".tgi[0].vh" -type "double2" 941.26980386713979 44.444442678380966 ;
createNode polyExtrudeFace -n "polyExtrudeFace5";
	rename -uid "53F424E0-4BD4-2681-BB82-92A214D25361";
	setAttr ".ics" -type "componentList" 24 "f[16]" "f[20]" "f[22]" "f[24]" "f[26]" "f[28]" "f[30]" "f[35:36]" "f[40]" "f[49]" "f[51]" "f[54]" "f[56]" "f[58]" "f[62]" "f[66]" "f[72]" "f[79:80]" "f[85]" "f[87]" "f[89]" "f[91]" "f[93]" "f[95]";
	setAttr ".ix" -type "matrix" 2.3222049723809342 0 0 0 0 2.3222049723809342 0 0 0 0 2.3222049723809342 0
		 0 3.679833024183659 -1.9702389424755185e-08 1;
	setAttr ".ws" yes;
	setAttr ".pvt" -type "float3" 1.3841421e-07 3.6798334 -0.59909892 ;
	setAttr ".rs" 53185;
	setAttr ".lt" -type "double3" 1.033895191682177e-15 9.5756735873919752e-16 0.1062446768995243 ;
	setAttr ".ls" -type "double3" 0.31999999961086151 0.31999999961086151 0.248962728961615 ;
	setAttr ".c[0]"  0 1 1;
	setAttr ".cbn" -type "double3" -2.8746650530839859 0.80516797109967309 -2.8746653496147805 ;
	setAttr ".cbx" -type "double3" 2.874665329912391 6.5544989077528601 1.676467540927987 ;
createNode polySoftEdge -n "polySoftEdge1";
	rename -uid "C6C9332C-4C5B-7051-AA5C-ADBF56730753";
	setAttr ".uopa" yes;
	setAttr ".ics" -type "componentList" 1 "e[*]";
	setAttr ".ix" -type "matrix" 2.3222049723809342 0 0 0 0 2.3222049723809342 0 0 0 0 2.3222049723809342 0
		 0 3.679833024183659 -1.9702389424755185e-08 1;
	setAttr ".a" 0;
createNode polyExtrudeFace -n "polyExtrudeFace4";
	rename -uid "C99CDCB8-4B76-D80D-67C6-7991011A7475";
	setAttr ".ics" -type "componentList" 24 "f[16]" "f[20]" "f[22]" "f[24]" "f[26]" "f[28]" "f[30]" "f[35:36]" "f[40]" "f[49]" "f[51]" "f[54]" "f[56]" "f[58]" "f[62]" "f[66]" "f[72]" "f[79:80]" "f[85]" "f[87]" "f[89]" "f[91]" "f[93]" "f[95]";
	setAttr ".ix" -type "matrix" 2.3222049723809342 0 0 0 0 2.3222049723809342 0 0 0 0 2.3222049723809342 0
		 0 3.679833024183659 -1.9702389424755185e-08 1;
	setAttr ".ws" yes;
	setAttr ".pvt" -type "float3" 0 3.6798334 -0.48793921 ;
	setAttr ".rs" 51279;
	setAttr ".lt" -type "double3" -1.0061396160665481e-15 -1.2628786905111156e-15 0.27837550922695642 ;
	setAttr ".ls" -type "double3" 0.24999999611805254 0.24999999611805254 0.24999999611805254 ;
	setAttr ".c[0]"  0 1 1;
	setAttr ".cbn" -type "double3" -2.6545911772325561 1.0252418469511029 -2.6545911969349456 ;
	setAttr ".cbx" -type "double3" 2.6545911772325561 6.3344247550730248 1.6787127577063246 ;
createNode polyExtrudeFace -n "polyExtrudeFace3";
	rename -uid "7BBB973E-4C9F-C5B8-D0FC-D39754442FB5";
	setAttr ".ics" -type "componentList" 24 "f[16]" "f[20]" "f[22]" "f[24]" "f[26]" "f[28]" "f[30]" "f[35:36]" "f[40]" "f[49]" "f[51]" "f[54]" "f[56]" "f[58]" "f[62]" "f[66]" "f[72]" "f[79:80]" "f[85]" "f[87]" "f[89]" "f[91]" "f[93]" "f[95]";
	setAttr ".ix" -type "matrix" 2.3222049723809342 0 0 0 0 2.3222049723809342 0 0 0 0 2.3222049723809342 0
		 0 3.679833024183659 -1.9702389424755185e-08 1;
	setAttr ".ws" yes;
	setAttr ".pvt" -type "float3" 0 3.6798329 -0.31714538 ;
	setAttr ".rs" 33680;
	setAttr ".lt" -type "double3" 2.0122792321330962e-16 2.7755575615628914e-16 0.38463830232746277 ;
	setAttr ".ls" -type "double3" 0.43333333888557102 0.43333333888557102 0.43333333888557102 ;
	setAttr ".c[0]"  0 1 1;
	setAttr ".cbn" -type "double3" -2.3686401690270498 1.3111928551566092 -2.3686401887294393 ;
	setAttr ".cbx" -type "double3" 2.3686401690270498 6.0484731932107092 1.7343494537045339 ;
createNode polyTweak -n "polyTweak1";
	rename -uid "BF3F308D-4180-7E49-1C4E-9987C485D11E";
	setAttr ".uopa" yes;
	setAttr -s 98 ".tk[0:97]" -type "float3"  -0.33667237 -0.33667237 0.33667234
		 0.33667237 -0.33667237 0.33667234 -0.33667237 0.33667237 0.33667222 0.33667237 0.33667237
		 0.33667231 -0.33667237 0.33667231 -0.33667243 0.33667237 0.33667231 -0.33667243 -0.33667237
		 -0.33667237 -0.33667237 0.33667237 -0.33667237 -0.33667243 -0.42524004 0.42524004
		 -3.6018442e-08 0.42524004 9.8511919e-09 -0.42524004 1.5392497e-09 -0.42524004 -0.42524004
		 -0.42524004 -1.6316037e-08 -0.42524004 0.42524004 -0.42524004 -2.9553588e-08 -0.42524004
		 -0.42524004 -3.3863492e-09 0 -0.42524004 0.42524004 0.42524004 -9.8511919e-09 0.42524004
		 1.5392497e-09 0.42524004 0.42524004 -0.42524004 7.5125893e-17 0.42524004 0.42524004
		 0.42524004 -9.8511919e-09 1.5392497e-09 0.42524004 -0.42524004 1.1082594e-08 -2.0318092e-08
		 0.58076006 1.0466892e-08 0.58076 -2.7706488e-08 1.0466892e-08 -8.0040969e-09 -0.58076006
		 1.0466892e-08 -0.58076 -1.1698297e-08 0.58076 -2.0318092e-08 -1.9086695e-08 -0.58076
		 -6.1569938e-10 -2.0318092e-08 -0.21577653 -0.21577655 0.505009 0.21577653 -0.21577653
		 0.505009 0.21577652 0.21577653 0.505009 -0.21577653 0.21577653 0.505009 -0.21577653
		 0.505009 0.21577649 0.21577653 0.505009 0.21577652 0.21577652 0.505009 -0.21577655
		 -0.21577655 0.505009 -0.21577655 -0.21577653 0.21577652 -0.505009 0.21577653 0.21577653
		 -0.505009 0.21577653 -0.21577655 -0.505009 -0.21577655 -0.21577653 -0.505009 -0.21577653
		 -0.505009 -0.21577653 0.21577653 -0.505009 -0.21577655 0.21577652 -0.505009 0.21577653
		 -0.21577655 -0.505009 0.21577652 0.505009 -0.21577653 0.21577653 0.505009 -0.21577653
		 -0.21577655 0.505009 0.21577655 -0.21577655 0.505009 0.21577653 0.21577649 -0.505009
		 -0.21577653 -0.21577655 -0.505009 -0.21577653 0.21577652 -0.505009 0.21577653 0.21577649
		 -0.505009 0.21577653 -0.21577653 -9.8511919e-09 -0.23184507 0.54173696 0.23184507
		 -9.8511919e-09 0.54173696 1.9702384e-08 0.23184507 0.54173696 -0.23184507 1.9702384e-08
		 0.54173684 -9.8511919e-09 0.54173696 0.23184507 0.23184507 0.54173696 -9.8511919e-09
		 1.9702384e-08 0.54173696 -0.23184507 -0.23184507 0.5417369 -3.9404767e-08 -9.8511919e-09
		 0.23184507 -0.54173696 0.23184507 9.8511919e-09 -0.54173696 1.9702384e-08 -0.23184507
		 -0.54173696 -0.23184507 -1.9702384e-08 -0.54173696 -9.8511919e-09 -0.54173696 -0.23184507
		 0.23184507 -0.54173696 -2.9553588e-08 1.9702384e-08 -0.54173696 0.23184507 -0.23184507
		 -0.5417369 0 0.54173696 -0.23184507 -9.8511919e-09 0.54173696 -9.8511919e-09 -0.23184507
		 0.54173696 0.23184507 -3.9404767e-08 0.5417369 1.9702384e-08 0.23184507 -0.54173696
		 -0.23184507 -2.9553588e-08 -0.54173696 -9.8511919e-09 0.23184507 -0.54173696 0.23184507
		 0 -0.5417369 1.9702384e-08 -0.23184507 -0.1997081 -0.39597276 0.39597276 0.1997081
		 -0.39597276 0.39597276 0.39597279 -0.1997081 0.39597276 0.39597276 0.19970807 0.39597276
		 0.1997081 0.39597276 0.39597276 -0.1997081 0.39597276 0.39597276 -0.39597276 0.19970807
		 0.39597276 -0.39597279 -0.1997081 0.39597276 0.39597279 0.39597276 0.19970807 0.39597276
		 0.39597276 -0.19970807 0.1997081 0.39597276 -0.39597279 -0.1997081 0.39597276 -0.39597279
		 -0.39597276 0.39597276 -0.1997081 -0.39597276 0.39597276 0.19970807 0.39597276 0.1997081
		 -0.39597279 0.39597276 -0.1997081 -0.39597279 0.1997081 -0.39597276 -0.39597279 -0.1997081
		 -0.39597276 -0.39597279 -0.39597276 -0.1997081 -0.39597279 -0.39597276 0.1997081
		 -0.39597279 0.39597279 -0.39597276 -0.1997081 0.39597276 -0.39597276 0.19970804 -0.39597276
		 -0.39597276 0.19970807 -0.39597276 -0.39597276 -0.1997081;
createNode polySmoothFace -n "polySmoothFace2";
	rename -uid "03A98E37-42A0-0092-5661-9CBCC295D6B3";
	setAttr ".ics" -type "componentList" 1 "f[*]";
	setAttr ".sdt" 2;
	setAttr ".dv" 2;
	setAttr ".suv" yes;
	setAttr ".ps" 0.10000000149011612;
	setAttr ".ro" 1;
	setAttr ".ma" yes;
	setAttr ".m08" yes;
createNode polyCube -n "polyCube2";
	rename -uid "F7A9661A-48CC-5C10-2CC9-2ABF81AF80F2";
	setAttr ".cuv" 4;
createNode polyMapDel -n "polyMapDel1";
	rename -uid "C8A484D9-4DE3-2013-20DB-E892A88414E0";
	setAttr ".uopa" yes;
	setAttr ".ics" -type "componentList" 1 "f[0:407]";
createNode polyPlanarProj -n "polyPlanarProj1";
	rename -uid "5EC04E13-40A0-3E8D-DD9A-3B9F231533F3";
	setAttr ".uopa" yes;
	setAttr ".ics" -type "componentList" 1 "f[0:407]";
	setAttr ".ix" -type "matrix" 2.3222049723809342 0 0 0 0 2.3222049723809342 0 0 0 0 2.3222049723809342 0
		 0 3.679833024183659 -1.9702389424755185e-08 1;
	setAttr ".ws" yes;
	setAttr ".pc" -type "double3" 0 3.6798331737518311 -0.29890429973602295 ;
	setAttr ".ps" -type "double2" 7.3253031726724922 7.3253043501669586 ;
	setAttr ".cam" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1;
createNode polyMapCut -n "polyMapCut1";
	rename -uid "59CA5F11-4A88-0529-CFA7-E09F98B7F335";
	setAttr ".uopa" yes;
	setAttr ".ics" -type "componentList" 16 "e[77]" "e[79]" "e[83]" "e[85]" "e[125]" "e[127]" "e[131]" "e[133]" "e[146]" "e[148]" "e[160]" "e[163]" "e[170]" "e[172]" "e[184]" "e[187]";
createNode polyTweakUV -n "polyTweakUV1";
	rename -uid "14990C3C-41E1-87B2-B77D-ACA9810CFF7F";
	setAttr ".uopa" yes;
	setAttr -s 426 ".uvtk";
	setAttr ".uvtk[0:249]" -type "float2" 0.27650094 -0.087792456 0.24179584
		 -0.059392337 0.25062913 -0.10489196 0.29465365 -0.11629945 0.19215426 -0.044184271
		 0.18909416 -0.095830739 0.18608943 -0.16515839 0.25498134 -0.16820377 0.30620044
		 -0.17232227 0.1370655 -0.05470768 0.12661067 -0.099178374 0.10925984 -0.071213245
		 0.083218627 -0.10476011 0.066610925 -0.15878326 0.11729167 -0.16207111 0.11991451
		 -0.22538257 0.18208489 -0.23393679 0.075730912 -0.2145949 0.091271989 -0.2428984
		 0.12548056 -0.27010149 0.17609826 -0.28465885 0.24602228 -0.22979319 0.29161775 -0.2222538
		 0.23422024 -0.27297169 0.26379561 -0.25608093 0.29770875 -0.33822346 0.29619735 -0.33956009
		 0.29548746 -0.34115636 0.29780364 -0.33886325 0.17122409 -0.28127745 0.22418752 -0.27251008
		 -0.38472131 -0.30631432 -0.31072924 -0.20608631 0.26381314 -0.25182563 0.234337 -0.22745711
		 0.13154975 -0.36356252 0.12880829 -0.36237317 0.1293216 -0.36344033 0.13234523 -0.36477667
		 0.093921535 -0.24254876 0.12591007 -0.26723424 0.065822951 -0.32546511 0.06370198
		 -0.32319775 0.06424319 -0.32311514 0.06659364 -0.3255116 -0.42071438 -0.34037158
		 -0.3976911 -0.38844404 -0.40156239 -0.38897446 -0.40119573 -0.38693371 -0.39758673
		 -0.38635811 0.11772273 -0.21287507 -0.41624397 -0.21314207 -0.38845438 -0.25724784
		 -0.43405101 -0.26772681 -0.43686962 -0.26656273 -0.43608087 -0.26434949 -0.43405312
		 -0.26452616 -0.33163044 -0.23435286 -0.3503935 -0.14518106 -0.28732538 -0.12242582
		 -0.19224116 -0.1944083 -0.19492573 -0.19755518 -0.1940178 -0.19715726 -0.19158801
		 -0.19420052 -0.26064724 -0.15633455 -0.25462618 -0.23342219 -0.25756454 -0.23454085
		 -0.25628278 -0.23216823 -0.25364056 -0.23122874 -0.18285987 -0.022132941 -0.2169379
		 -0.060637888 -0.16719484 0.0072183311 -0.16903377 0.077973142 -0.11409128 0.12890846
		 -0.24733976 0.031495575 -0.20901424 0.20341277 -0.12079599 0.24521323 -0.061595857
		 0.16418645 -0.064809889 0.16260776 -0.064280927 0.16801938 -0.060845733 0.16928348
		 -0.31890982 -0.11458094 -0.32151964 -0.11420496 -0.32056487 -0.11044982 -0.31765655
		 -0.11055258 -0.38028371 -0.13204442 -0.37740552 -0.029300116 -0.31767994 -0.00055031478
		 -0.3533923 0.12338087 -0.28806406 0.15835094 -0.24105561 0.28178653 -0.24515492 0.28050432
		 -0.2446636 0.28597721 -0.240843 0.28756681 -0.30153617 0.28050181 -0.24025291 0.32693967
		 -0.24765843 0.39740869 -0.19765571 0.46883836 -0.16596898 0.37974712 -0.12707011
		 0.52470136 -0.059356838 0.28796872 -0.091724455 0.40571025 -0.035046577 0.43448254
		 -0.064703092 0.54585183 -0.024978273 0.53549421 -0.025138669 0.66123378 0.0004548952
		 0.616355 -0.054047465 0.680246 -0.057422712 0.67962897 -0.057412669 0.68317449 -0.053753793
		 0.6836431 -0.084399417 0.63868266 0.18839762 -0.096967965 0.0068503525 0.7422033
		 0.054260746 0.7660799 0.051729485 0.76720989 0.051610701 0.76910728 0.054358527 0.76780969
		 -0.16267745 0.56456983 -0.23950526 0.4319599 -0.2293424 0.60762924 -0.23174334 0.60472226
		 -0.23107833 0.60498476 -0.22842419 0.60807961 0.16573754 -0.093596309 0.15736344
		 0.02894583 0.15424606 0.027694311 0.15363231 0.027984973 0.15648261 0.029200044 -0.19530427
		 0.45418105 0.10930335 -0.074585617 0.14585403 -0.052648906 0.076447777 0.014551844
		 0.074127726 0.012221511 0.074237101 0.011609849 0.075754277 0.012979861 0.19091865
		 -0.046045337 0.013632726 0.6928038 0.23430172 -0.060984604 0.27214777 -0.088816553
		 0.23521015 0.034988698 0.23222479 0.036278035 0.23308632 0.035014745 0.2357876 0.033874299
		 0.076720111 -0.11019342 0.12093685 -0.12496287 -0.0064929575 -0.11415343 -0.0061292499
		 -0.11469142 -0.0074865073 -0.11785001 -0.0078092664 -0.1170457 0.063008897 -0.15306288
		 -0.31292132 0.26482895 -0.37318277 0.086163744 -0.33836329 0.040627934 -0.48274595
		 -0.044002436 -0.48220065 -0.042686366 -0.48193964 -0.048183404 -0.48246592 -0.049825035
		 -0.4112702 -0.085502774 -0.40337965 -0.15588811 0.073713712 -0.20366538 0.0074415319
		 -0.27566442 0.0058281533 -0.27494907 0.0081068389 -0.27725899 0.00876249 -0.27717009
		 0.058093868 0.57198715 0.059506588 0.57422483 0.06102965 0.56976557 0.05866342 0.56834054
		 0.25407574 -0.14516783 -0.0082065761 0.49529067 -0.069148034 0.38137075 -0.039478481
		 0.32975182 0.29582453 -0.12574655 0.36038506 -0.05340815 0.35979235 -0.053524826
		 0.36118323 -0.055046204 0.36273503 -0.05573434 0.30355191 -0.17255288 0.3761282 -0.13503748
		 0.37642568 -0.1344246 0.37764537 -0.13728562 0.37738192 -0.13816589 0.28912044 -0.21870831
		 -0.14923561 0.16141304 0.38669306 -0.21384531 0.38544381 -0.21468803 0.38429183 -0.21740267
		 0.38539332 -0.21684498 0.34683019 -0.27785283 0.34677267 -0.27864844 0.34453875 -0.28077942
		 0.34448326 -0.28020877 -0.10514548 0.14827475 0.011967119 0.24195069 0.011395987
		 0.24203157 0.0093416572 0.23694909 0.0099844001 0.23721993 -0.064617693 0.019439612
		 -0.063582122 0.018782947 -0.06664139 0.014653239 -0.066698223 0.016153309 0.26426566
		 -0.30406362 0.28017002 -0.29379147 0.2592923 -0.31137067 0.2823599 -0.2956838 0.12662402
		 -0.31331593 0.15266779 -0.32158327 0.127103 -0.3170411 0.15409276 -0.32608956 0.079535358
		 -0.27797142 0.097505085 -0.29305306 0.086601608 -0.26837686 0.10369002 -0.28204337
		 -0.4113723 -0.35680982 -0.38392329 -0.34620455 -0.39844409 -0.32558575 -0.36882764
		 -0.31699249 -0.42678866 -0.236357 -0.40545946 -0.2552377 -0.41218656 -0.20532572
		 -0.39438891 -0.21059933 -0.24598125 -0.18690062 -0.20956105 -0.1276311 -0.22842854
		 -0.17157841 -0.19478431 -0.11934982 -0.29032886 -0.22177172 -0.25617236 -0.19510448
		 -0.27273867 -0.17954016 -0.2408143 -0.15716374 -0.11299393 0.13139248 -0.083676815
		 0.15175697 -0.09611246 0.2013931 -0.065162748 0.21787703 -0.33217311 -0.11802624
		 -0.30213571 -0.11341144 -0.31979996 -0.060661014 -0.28636214 -0.051095389 -0.26149243
		 0.23049402 -0.2223849 0.24791452 -0.24523255 0.30113003 -0.20816523 0.32222691 -0.087809041
		 0.6100229 -0.056204498 0.61795497 -0.07524848 0.65707171 -0.042302772 0.66430789
		 0.014045049 0.71782857 0.03097295 0.69896305 0.022176906 0.74833834 0.037217312 0.72760189;
	setAttr ".uvtk[250:425]" -0.22823617 0.52964854 -0.19501567 0.58128661 -0.21364325
		 0.53904891 -0.17777435 0.59740978 0.15573725 -0.031342089 0.17705289 -0.027558267
		 0.14994076 -0.017824702 0.17371312 -0.012381902 0.08982908 -0.030976377 0.11210833
		 -0.014831563 0.09211468 -0.033044808 0.10756736 -0.022673866 0.20965916 -0.0020670798
		 0.23609111 -0.011215707 0.21318945 -0.0072077122 0.23876175 -0.015428593 0.054857004
		 -0.11559141 0.040163636 -0.11169754 0.047875762 -0.13640624 0.032197133 -0.13487294
		 -0.42796415 0.0096665546 -0.42095679 -0.0088458136 -0.43986002 -0.062620252 -0.43773615
		 -0.08829537 0.035424709 -0.23875195 0.043425202 -0.24396279 0.050894365 -0.26158041
		 0.053303227 -0.25957346 0.02605217 0.58524787 0.013485672 0.55411822 0.031179911
		 0.53300935 0.013620967 0.51071656 0.31500864 -0.0712623 0.31675279 -0.0691154 0.32587838
		 -0.086840615 0.33342177 -0.091549218 0.3306489 -0.13044167 0.31681263 -0.13611007
		 0.33640575 -0.15480149 0.32065761 -0.15817618 0.34326118 -0.19427353 0.3481034 -0.19136953
		 0.33513021 -0.22010729 0.33910841 -0.21847689 0.31447941 -0.24700603 0.30317569 -0.2403484
		 0.29934013 -0.26538068 0.28977394 -0.25805432 -0.031373143 0.29446384 -0.020607382
		 0.27988467 -0.061537534 0.21068904 -0.047494859 0.20591688 -0.085608393 0.071749821
		 -0.093466967 0.068683475 -0.1194059 0.0067159832 -0.11684898 0.02066109 0.28786212
		 -0.32945675 0.2922405 -0.32607812 0.2862004 -0.33262855 0.2927649 -0.32730705 0.1289365
		 -0.3488515 0.13641647 -0.35169297 0.12963369 -0.35088199 0.13774267 -0.35398489 0.06882035
		 -0.31018272 0.074390762 -0.31552616 0.070381872 -0.30871382 0.076412492 -0.31411961
		 -0.40303138 -0.37806633 -0.393444 -0.37608877 -0.40055454 -0.37103072 -0.39132226
		 -0.36910191 -0.43293294 -0.25756696 -0.42593664 -0.26189807 -0.42976508 -0.25025159
		 -0.42432451 -0.25120685 -0.20669407 -0.19074658 -0.19775963 -0.17856681 -0.20299843
		 -0.18818787 -0.19490919 -0.17697835 -0.26461753 -0.22759381 -0.25563449 -0.22244588
		 -0.26034462 -0.21842197 -0.2522136 -0.21407032 -0.076178014 0.15785334 -0.067663699
		 0.1628606 -0.073066294 0.17505369 -0.064092666 0.17906129 -0.32266238 -0.11196361
		 -0.31481048 -0.11190505 -0.31963852 -0.09875384 -0.31087932 -0.097548127 -0.24739915
		 0.27054998 -0.23633188 0.27470538 -0.24443161 0.28794369 -0.23398787 0.29306224 -0.063959196
		 0.66287351 -0.054809034 0.66479069 -0.062178746 0.67435682 -0.052411675 0.67589313
		 0.042068657 0.75419492 0.048086762 0.74996054 0.042922586 0.76105416 0.049244165
		 0.75627106 -0.22836924 0.58746785 -0.22039708 0.59853423 -0.22552732 0.58926928 -0.21672556
		 0.6012826 0.1547828 0.012023423 0.162532 0.014363531 0.15335801 0.014311817 0.16071543
		 0.01679584 0.079177313 0.00063031539 0.085661642 0.0060715042 0.079751424 -0.00055783056
		 0.084097616 0.0029056929 0.22670802 0.025501069 0.23471841 0.022501674 0.22839305
		 0.023072537 0.23576245 0.020347949 0.0098253619 -0.11510658 0.0073062293 -0.114049
		 0.0069451444 -0.12285273 0.0043447129 -0.12140381 -0.46762502 -0.032461219 -0.46751714
		 -0.036915444 -0.46881822 -0.05003532 -0.46914008 -0.055712428 0.014343156 -0.26564866
		 0.017604962 -0.26735201 0.01961197 -0.27219114 0.020917431 -0.27170217 0.049647331
		 0.57385206 0.045926034 0.56649685 0.052534372 0.56050515 0.046883911 0.55541795 0.34760833
		 -0.05903754 0.34873331 -0.058449868 0.35115671 -0.063394606 0.35426342 -0.064950347
		 0.36275673 -0.13411754 0.36042005 -0.13552463 0.36528069 -0.14155364 0.36277765 -0.14336455
		 0.37351954 -0.20992535 0.3758958 -0.20835096 0.37077475 -0.21734834 0.37288874 -0.21643603
		 0.33685142 -0.27010685 0.3354404 -0.26801974 0.33154035 -0.27573365 0.33009493 -0.27411228
		 -0.0013022721 0.25169381 0.0010310709 0.24966475 -0.0080573298 0.2333653 -0.005385844
		 0.23290384 -0.070985913 0.030538175 -0.073438853 0.03092346 -0.079788685 0.015830908
		 -0.079620719 0.01984534 -0.22777084 -0.040959783 0.24576098 -0.20681611 0.25070062
		 -0.17033947 -0.011982288 0.56079733 0.2499184 -0.11103269 -0.43190008 -0.28344181
		 0.10797542 -0.18313581 0.11456213 -0.14527962 -0.27424341 0.25572059 0.13264611 -0.096478015
		 -0.11390007 0.61631173 -0.040112972 0.71318966 0.22122064 -0.097185433 0.13607135
		 -0.22522253 0.16792217 -0.2344684 0.20450881 -0.23870689;
select -ne :time1;
	setAttr ".o" 1;
	setAttr ".unw" 1;
select -ne :hardwareRenderingGlobals;
	setAttr ".otfna" -type "stringArray" 22 "NURBS Curves" "NURBS Surfaces" "Polygons" "Subdiv Surface" "Particles" "Particle Instance" "Fluids" "Strokes" "Image Planes" "UI" "Lights" "Cameras" "Locators" "Joints" "IK Handles" "Deformers" "Motion Trails" "Components" "Hair Systems" "Follicles" "Misc. UI" "Ornaments"  ;
	setAttr ".otfva" -type "Int32Array" 22 0 1 1 1 1 1
		 1 1 1 0 0 0 0 0 0 0 0 0
		 0 0 0 0 ;
	setAttr ".fprt" yes;
	setAttr ".rtfm" 1;
select -ne :renderPartition;
	setAttr -s 2 ".st";
select -ne :renderGlobalsList1;
select -ne :defaultShaderList1;
	setAttr -s 5 ".s";
select -ne :postProcessList1;
	setAttr -s 2 ".p";
select -ne :defaultRenderingList1;
select -ne :standardSurface1;
	setAttr ".bc" -type "float3" 0.40000001 0.40000001 0.40000001 ;
	setAttr ".sr" 0.5;
select -ne :initialShadingGroup;
	setAttr -s 2 ".dsm";
	setAttr ".ro" yes;
select -ne :initialParticleSE;
	setAttr ".ro" yes;
select -ne :defaultRenderGlobals;
	addAttr -ci true -h true -sn "dss" -ln "defaultSurfaceShader" -dt "string";
	setAttr ".ren" -type "string" "arnold";
	setAttr ".dss" -type "string" "standardSurface1";
select -ne :defaultResolution;
	setAttr ".pa" 1;
select -ne :defaultColorMgtGlobals;
	setAttr ".cfe" yes;
	setAttr ".cfp" -type "string" "<MAYA_RESOURCES>/OCIO-configs/Maya2022-default/config.ocio";
	setAttr ".vtn" -type "string" "ACES 1.0 SDR-video (sRGB)";
	setAttr ".vn" -type "string" "ACES 1.0 SDR-video";
	setAttr ".dn" -type "string" "sRGB";
	setAttr ".wsn" -type "string" "ACEScg";
	setAttr ".otn" -type "string" "ACES 1.0 SDR-video (sRGB)";
	setAttr ".potn" -type "string" "ACES 1.0 SDR-video (sRGB)";
select -ne :hardwareRenderGlobals;
	setAttr ".ctrs" 256;
	setAttr ".btrs" 512;
connectAttr ":defaultColorMgtGlobals.cme" "imagePlaneShape1.cme";
connectAttr ":defaultColorMgtGlobals.cfe" "imagePlaneShape1.cmcf";
connectAttr ":defaultColorMgtGlobals.cfp" "imagePlaneShape1.cmcp";
connectAttr ":defaultColorMgtGlobals.wsn" "imagePlaneShape1.ws";
connectAttr ":frontShape.msg" "imagePlaneShape1.ltc";
connectAttr "polyTweakUV1.out" "pCubeShape3.i";
connectAttr "polyTweakUV1.uvtk[0]" "pCubeShape3.uvst[0].uvtw";
relationship "link" ":lightLinker1" ":initialShadingGroup.message" ":defaultLightSet.message";
relationship "link" ":lightLinker1" ":initialParticleSE.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" ":initialShadingGroup.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" ":initialParticleSE.message" ":defaultLightSet.message";
connectAttr "layerManager.dli[0]" "defaultLayer.id";
connectAttr "renderLayerManager.rlmi[0]" "defaultRenderLayer.rlid";
connectAttr ":defaultArnoldDisplayDriver.msg" ":defaultArnoldRenderOptions.drivers"
		 -na;
connectAttr ":defaultArnoldFilter.msg" ":defaultArnoldRenderOptions.filt";
connectAttr ":defaultArnoldDriver.msg" ":defaultArnoldRenderOptions.drvr";
connectAttr "polySoftEdge1.out" "polyExtrudeFace5.ip";
connectAttr "pCubeShape3.wm" "polyExtrudeFace5.mp";
connectAttr "polyExtrudeFace4.out" "polySoftEdge1.ip";
connectAttr "pCubeShape3.wm" "polySoftEdge1.mp";
connectAttr "polyExtrudeFace3.out" "polyExtrudeFace4.ip";
connectAttr "pCubeShape3.wm" "polyExtrudeFace4.mp";
connectAttr "polyTweak1.out" "polyExtrudeFace3.ip";
connectAttr "pCubeShape3.wm" "polyExtrudeFace3.mp";
connectAttr "polySmoothFace2.out" "polyTweak1.ip";
connectAttr "polyCube2.out" "polySmoothFace2.ip";
connectAttr "polyExtrudeFace5.out" "polyMapDel1.ip";
connectAttr "polyMapDel1.out" "polyPlanarProj1.ip";
connectAttr "pCubeShape3.wm" "polyPlanarProj1.mp";
connectAttr "polyPlanarProj1.out" "polyMapCut1.ip";
connectAttr "polyMapCut1.out" "polyTweakUV1.ip";
connectAttr "defaultRenderLayer.msg" ":defaultRenderingList1.r" -na;
connectAttr "pCubeShape1.iog" ":initialShadingGroup.dsm" -na;
connectAttr "pCubeShape3.iog" ":initialShadingGroup.dsm" -na;
// End of Buggy.ma
