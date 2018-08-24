using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;

namespace TestEnvironment.Docker.Compose
{
    internal class SkipIfMissedBackend : DefaultObjectSerializerBackend
    {
        public override string ReadMemberName(ref ObjectContext objectContext, string memberName, out bool skipMember)
        {
            var readMemberName = base.ReadMemberName(ref objectContext, memberName, out skipMember);

            skipMember = !objectContext.Descriptor.Contains(readMemberName);

            return readMemberName;
        }
    }
}
