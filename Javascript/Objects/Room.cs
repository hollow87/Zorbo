using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Room : ObjectInstance
    {
        Config config = null;

        JScript script = null;
        IServer server = null;

        RoomStats stats;
        Admin admins;
        Records records;
        Banned banned;
        DnsBanned dnsbans;
        RangeBanned rangebans;
        FloodRules floodRules;

        List users = null;
        ReadOnlyList pusers = null;
        
        protected override string InternalClassName {
            get { return "Room"; }
        }

        public Room(JScript script, IServer server)
            : base(script.Engine) {

            this.script = script;
            this.server = server;

            this.users = new List(script);
            this.pusers = new ReadOnlyList(script, users);

            this.stats = new RoomStats(script, server.Stats);
            this.records = new Records(script, server.History);
            this.admins = new Admin(script, server.History.Admin);
            this.banned = new Banned(script, server.History);
            this.dnsbans = new DnsBanned(script, server.History);
            this.rangebans = new RangeBanned(script, server.History);
            this.floodRules = new FloodRules(script, server.FloodRules);

            this.PopulateFields();
            this.PopulateFunctions();
        }

        [JSProperty(Name = "topic")]
        public string Topic {
            get { return this.server.Config.Topic; }
            set { this.server.Config.Topic = value; }
        }

        [JSProperty(Name = "users")]
        public ReadOnlyList Users {
            get { return pusers; }
        }

        [JSProperty(Name = "running")]
        public Boolean Running {
            get { return this.server.Running; }
        }

        [JSProperty(Name = "localIp")]
        public String LocalIp {
            get { return this.server.LocalIp.ToString(); }
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIp {
            get { return this.server.ExternalIp.ToString(); }
        }

        [JSProperty(Name = "config")]
        public Config Config {
            get {
                if (config == null)
                    config = new Config(script, server.Config);

                return config;
            }
        }

        [JSProperty(Name = "stats", IsEnumerable = true)]
        public RoomStats Stats {
            get { return stats; }
        }

        [JSProperty(Name = "records", IsEnumerable = true)]
        public Records Records {
            get { return records; }
        }

        [JSProperty(Name = "admin", IsEnumerable = true)]
        public Admin Admins {
            get { return admins; }
        }

        [JSProperty(Name = "bans", IsEnumerable = true)]
        public Banned Bans {
            get { return banned; }
        }

        [JSProperty(Name = "dnsBans", IsEnumerable = true)]
        public DnsBanned DnsBans {
            get { return dnsbans; }
        }

        [JSProperty(Name = "rangeBans", IsEnumerable = true)]
        public RangeBanned RangeBans {
            get { return rangebans; }
        }

        [JSProperty(Name = "floodRules", IsEnumerable = true)]
        public FloodRules FloodRules {
            get { return floodRules; }
        }

        public User FindUser(object search) {

            if (search is Null)
                return null;

            if (search is Int32) {

                int id = (int)search;
                return (User)this.script.Room.Users.Items.Find((s) => ((User)s).Id == id);
            }
            else if (search is NumberInstance) {

                int id = (int)((NumberInstance)search).Value;
                return (User)this.script.Room.Users.Items.Find((s) => ((User)s).Id == id);
            }
            else if (search is String || search is ConcatenatedString) {

                string id = search.ToString();
                return (User)this.script.Room.Users.Items.Find((s) => ((User)s).Name.StartsWith(id));
            }
            else if (search is FunctionInstance) {

                var match = (FunctionInstance)search;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    Object ret = null;
                    User user = (User)this.script.Room.Users.Items[i];

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret))
                            return user;
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }

            return null;
        }

        [JSFunction(Name = "sendAnnounce", IsEnumerable = true, IsWritable = false)]
        public void SendAnnounce(object a, object b) {

            if (++script.Counters["print"] > 100)
                throw new JavaScriptException(Engine.Error.Construct("Send announce limit reached"), 0, null);

            if (b is Undefined)
                server.SendAnnounce(a.ToString());

            else if (a is User)
                server.SendAnnounce(((User)a).Client, b.ToString());

            else if (a is Int32)
                server.SendAnnounce((s) => s.Vroom == (int)a, b.ToString());
            else if (a is NumberInstance) {

                int id = (int)((NumberInstance)a).Value;
                server.SendAnnounce((s) => s.Vroom == (int)a, b.ToString());
            }
            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret))
                            server.SendAnnounce(user.Client, b.ToString());
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }

        [JSFunction(Name = "sendText", IsEnumerable = true, IsWritable = false)]
        public void SendText(object a, object b, object c) {

            if (++script.Counters["text"] > 100)
                throw new JavaScriptException(Engine.Error.Construct("Send text limit reached"), 0, null);

            if (c is Undefined) {
                if (a is String || a is ConcatenatedString)
                    server.SendText(a.ToString(), b.ToString());

                else if (a is User)
                    server.SendText(((User)a).Client, b.ToString());
            }
            else if (a is String || a is ConcatenatedString)
                server.SendText(a.ToString(), b.ToString(), c.ToString());

            else if (a is User) {
                if (b is String || b is ConcatenatedString)
                    server.SendText(((User)a).Client, b.ToString(), c.ToString());

                else if (b is User)
                    server.SendText(((User)a).Client, ((User)b).Client, c.ToString());
            }
            else if (a is Int32) {
                if (b is String || b is ConcatenatedString)
                    server.SendText((s) => s.Vroom == (int)a, b.ToString(), c.ToString());

                else if (b is User)
                    server.SendText((s) => s.Vroom == (int)a, ((User)b).Client, c.ToString());
            }
            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret)) {
                            if (b is String || b is ConcatenatedString)
                                server.SendText(user.Client, b.ToString(), c.ToString());

                            else if (b is User)
                                server.SendText(user.Client, ((User)b).Client, c.ToString());
                        }
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }

        [JSFunction(Name = "sendEmote", IsEnumerable = true, IsWritable = false)]
        public void SendEmote(object a, object b, object c) {

            if (++script.Counters["emote"] > 100)
                throw new JavaScriptException(Engine.Error.Construct("Send emote limit reached"), 0, null);

            if (c is Undefined) {
                if (a is String || a is ConcatenatedString)
                    server.SendEmote(a.ToString(), b.ToString());

                else if (a is User)
                    server.SendEmote(((User)a).Client, b.ToString());
            }
            else if (a is String || a is ConcatenatedString)
                server.SendEmote(a.ToString(), b.ToString(), c.ToString());

            else if (a is User) {
                if (b is String || b is ConcatenatedString)
                    server.SendEmote(((User)a).Client, b.ToString(), c.ToString());

                else if (b is User)
                    server.SendEmote(((User)a).Client, ((User)b).Client, c.ToString());
            }
            else if (a is Int32) {
                if (b is String || b is ConcatenatedString)
                    server.SendEmote((s) => s.Vroom == (int)a, b.ToString(), c.ToString());

                else if (b is User)
                    server.SendEmote((s) => s.Vroom == (int)a, ((User)b).Client, c.ToString());
            }
            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret)) {
                            if (b is String || b is ConcatenatedString)
                                server.SendEmote(user.Client, b.ToString(), c.ToString());

                            else if (b is User)
                                server.SendEmote(user.Client, ((User)b).Client, c.ToString());
                        }
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }

        [JSFunction(Name = "sendPrivate", IsEnumerable = true, IsWritable = false)]
        public void SendPrivate(object a, object b, object c) {

            if (++script.Counters["private"] > 150)
                throw new JavaScriptException(Engine.Error.Construct("Send PM limit reached"), 0, null);

            if (c is Undefined) 
                return;

            if (a is Undefined || b is Undefined)
                return;

            if (a is String || a is ConcatenatedString)
                server.SendPrivate(a.ToString(), b.ToString(), c.ToString());

            else if (a is User) {
                if (b is String || b is ConcatenatedString)
                    server.SendPrivate(((User)a).Client, b.ToString(), c.ToString());
                
                else if (b is User)
                    server.SendPrivate(((User)a).Client, ((User)b).Client, c.ToString());
            }
            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret)) {
                            if (b is String || b is ConcatenatedString)
                                server.SendPrivate(user.Client, b.ToString(), c.ToString());

                            else if (b is User)
                                server.SendPrivate(user.Client, ((User)b).Client, c.ToString());
                        }
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }

        [JSFunction(Name = "sendWebsite", IsEnumerable = true, IsWritable = false)]
        public void SendWebSite(object a, object b, object c) {

            if (++script.Counters["website"] > 100)
                throw new JavaScriptException(Engine.Error.Construct("Send website limit reached"), 0, null);

            if (c is Undefined)
                server.SendWebsite(a.ToString(), b.ToString());

            else if (a is String || a is ConcatenatedString)
                server.SendWebsite(a.ToString(), b.ToString(), c.ToString());

            else if (a is User)
                server.SendWebsite(((User)a).Client, b.ToString(), c.ToString());

            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret))
                            server.SendWebsite(user.Client, b.ToString(), c.ToString());
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }

        [JSFunction(Name = "sendHtml", IsEnumerable = true, IsWritable = false)]
        public void SendHtml(object a, object b) {

            if (++script.Counters["html"] > 100)
                throw new JavaScriptException(Engine.Error.Construct("Send html limit reached"), 0, null);

            if (b is Undefined)
                server.SendHtml(a.ToString());
            
            else if (a is String || a is ConcatenatedString)
                server.SendHtml(a.ToString(), b.ToString());

            else if (a is User)
                server.SendHtml(((User)a).Client, b.ToString());

            else if (a is FunctionInstance) {
                var match = (FunctionInstance)a;

                for (int i = 0; i < this.script.Room.Users.Items.Count; i++) {
                    User user = (User)this.script.Room.Users.Items[i];
                    Object ret = null;

                    try {
                        ret = match.Call(this, user);

                        if (TypeConverter.ConvertTo<bool>(Engine, ret))
                            server.SendHtml(user.Client, b.ToString());
                    }
                    catch (JavaScriptException jex) {
                        Jurassic.Self.OnError(jex);
                    }
                }
            }
        }
    }
}
