using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;


public interface IDbContext{
   ISession Open();
}

public  class NHibernateHelper : IDbContext

{
    private ISessionFactory _sessionFactory;
    private readonly IConfiguration _config;

    public NHibernateHelper(IConfiguration config){
      _config = config;
    }
    private  ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
              _sessionFactory =  InitializeSessionFactory();
            return _sessionFactory;
        }
    }

    private  ISessionFactory InitializeSessionFactory()
    {
        var _dbconfig = _config.GetSection("Db").Get<DbConfiguration>();
        
        return Fluently.Configure()

           .Database(
               PostgreSQLConfiguration.Standard
                       .ConnectionString(c =>
                           c.Host(_dbconfig.Host)
                           .Port(_dbconfig.Port)
                           .Database(_dbconfig.Database)
                           .Username(_dbconfig.User)
                           .Password(_dbconfig.Password))

           )

           .Mappings(m =>

                     m.FluentMappings

                         .AddFromAssemblyOf<Product>())

           .ExposeConfiguration(cfg => new SchemaExport(cfg)

            .Create(_dbconfig.Recreate, _dbconfig.Recreate))

           .BuildSessionFactory();
    }

    public  ISession Open()
    {
        return SessionFactory.OpenSession();
    }



}