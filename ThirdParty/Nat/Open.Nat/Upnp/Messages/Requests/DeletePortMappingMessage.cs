//
// Authors:
//   Alan McGovern  alan.mcgovern@gmail.com
//   Lucas Ontivero lucas.ontivero@gmail.com
//
// Copyright (C) 2006 Alan McGovern
// Copyright (C) 2014 Lucas Ontivero
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using Paulov.UnityBepInExNetworking.ThirdParty.Nat.Open.Nat;
using Paulov.UnityBepInExNetworking.ThirdParty.Nat.Open.Nat.Enums;
using Paulov.UnityBepInExNetworking.ThirdParty.Nat.Open.Nat.Upnp;

namespace Paulov.UnityBepInExNetworking.ThirdParty.Nat.Open.Nat.Upnp.Messages.Requests
{
    internal class DeletePortMappingRequestMessage : RequestMessageBase
    {
        private readonly Mapping _mapping;

        public DeletePortMappingRequestMessage(Mapping mapping)
        {
            _mapping = mapping;
        }

        public override IDictionary<string, object> ToXml()
        {
            return new Dictionary<string, object>
                       {
                           {"NewRemoteHost", string.Empty},
                           {"NewExternalPort", _mapping.PublicPort},
                           {"NewProtocol", _mapping.Protocol == Protocol.Tcp ? "TCP" : "UDP"}
                       };
        }
    }
}