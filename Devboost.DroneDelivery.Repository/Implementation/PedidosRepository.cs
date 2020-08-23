﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devboost.DroneDelivery.Domain.Entities;
using Devboost.DroneDelivery.Domain.Enums;
using Devboost.DroneDelivery.Domain.Interfaces.Repository;
using Devboost.DroneDelivery.Repository.Models;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace Devboost.DroneDelivery.Repository.Implementation
{
    public class PedidosRepository : IPedidosRepository
    {
        private readonly string _configConnectionString = "DroneDelivery";
        private readonly IDbConnectionFactoryExtended _dbFactory; 

        public PedidosRepository(IConfiguration config)
        {
            _dbFactory = new OrmLiteConnectionFactory(
                config.GetConnectionString(_configConnectionString),  
                SqlServerDialect.Provider);
        }

        public async Task<List<PedidoEntity>> GetAll()
        {
            using var conexao = await _dbFactory.OpenAsync();

            var list = await conexao.SelectAsync<Pedido>();

            return list.ConvertTo<List<PedidoEntity>>();

        }

        public async Task<PedidoEntity> GetByDroneID(Guid droneId)
        {
            using var conexao = await _dbFactory.OpenAsync();
            conexao.CreateTableIfNotExists<Pedido>();
            var p = await conexao.SingleAsync<Pedido>(
                p => 
                    p.DroneId == droneId 
                    && p.Status == PedidoStatus.EmTransito.ToString());

            return p.ConvertTo<PedidoEntity>();
            
        }

        public async Task Inserir(PedidoEntity pedido)
        {
            var model = pedido.ConvertTo<Pedido>();
            using var conexao = await _dbFactory.OpenAsync();
            
            conexao.CreateTableIfNotExists<Pedido>();
            await conexao.InsertAsync(model);

        }

        public async Task Atualizar(PedidoEntity pedido)
        {
            var model = pedido.ConvertTo<Pedido>();
            using var conexao = await _dbFactory.OpenAsync();
            
             conexao.CreateTableIfNotExists<Pedido>();
             await conexao.UpdateAsync(model);
         
        }

        private List<PedidoEntity> ConvertListModelToModelEntity(IEnumerable<Pedido> listPedido)
        {
            return listPedido.Select(ConvertModelToModelEntity).ToList();
        }

        private static PedidoEntity ConvertModelToModelEntity(Pedido pedido)
        {

            var p = new PedidoEntity
            {
                Id = pedido.Id,
                Status = Enum.Parse<PedidoStatus>(pedido.Status),
                DroneId = pedido.DroneId,
                DataHora = pedido.DataHora,
                Latitude = pedido.Latitude,
                Longitude = pedido.Longitude,
                PesoGramas = pedido.Peso
            };
            
            return p;

        }

    }
}