%module TupleGenerator
typedef unsigned int uint;

%{
#include "common/datatype/cDataType.h"
#include "common/datatype/cDTDescriptor.h"
#include "common/datatype/tuple/cTuple.h"
#include "common/datatype/tuple/cSpaceDescriptor.h"
#include "test/paged/tuplesgenerator_test/CollectionGeneratorBase.h"
#include "test/paged/tuplesgenerator_test/cCollectionGenerator.h"
%}

namespace common {
    namespace compression {}
    namespace datatype {}
    namespace utils {}
}

%include "std_string.i"
%include "std_wstring.i"
%include "std_vector.i"

%include "common/datatype/cDataType.h"
%include "common/datatype/cDTDescriptor.h"
%include "common/datatype/tuple/cTuple.h"
%include "common/datatype/tuple/cSpaceDescriptor.h"
%include "test/paged/tuplesgenerator_test/CollectionGeneratorBase.h"
%include "test/paged/tuplesgenerator_test/cCollectionGenerator.h"

%template(vector_uint) std::vector<uint>;
%template(vector_cTuple_ptr) std::vector<cTuple*>;
%template(vector_CartesianRangeInterval) std::vector<CartesianRangeInterval>;
%template(CollectionGenerator_cUInt) cCollectionGenerator<cUInt, cTuple>;
%template(CollectionGenerator_cInt) cCollectionGenerator<cInt, cTuple>;
%template(CollectionGenerator_cFloat) cCollectionGenerator<cFloat, cTuple>;
%template(CollectionGenerator_cDouble) cCollectionGenerator<cDouble, cTuple>;